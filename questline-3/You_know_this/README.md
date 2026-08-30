# You_know_this — Recursive Archive Analysis

- **Repository:** `ACMRecruitment-prabhasiva456`
- **Category:** Miscellaneous
- **Challenge:** [You_know_this](https://ctfd-production-10bf.up.railway.app/challenges#You_know_this-2)
- **Recovered token:** `ACM{too__eazyy_right}`

## Objective

Recover the original payload from nested archives by identifying each format from its contents rather than trusting its filename. Inspect every archive member, including hidden files, to avoid decoys.

## Input identification

The analyzed attachment, `txtpyc`, is 10,240 bytes. Although the supplied narrative mentions raw hex, this copy is already a binary TAR archive; it does not require an initial hex conversion.

SHA-256:

```text
f6b99e6d3a3588182d45172ae23716d98b4a2492b72dd67a5dc31d1d7b18d8ef
```

The TAR parser recognizes the archive and lists `task.bz2`. The solver then detects each successive format using signatures or archive validation:

| Format | Identification |
| --- | --- |
| bzip2 | `42 5a 68` (`BZh`) |
| gzip | `1f 8b` |
| ZIP | ZIP validation; this file begins `50 4b 03 04` |
| TAR | TAR header parsing and validation |

## Extraction

The first four operations reveal a ZIP archive:

```text
txtpyc [TAR]
  -> task.bz2 [bzip2]
  -> gzip
  -> ZIP
```

The ZIP contains both `challenge.tar` and `.heheh`. Following only the obvious filename would miss the token.

### Decoy branch

```text
challenge.tar [TAR]
  -> flag
  -> (bzip2 -> gzip) repeated 5 times
  -> "sorry wrong way\n"
```

Despite being named `flag`, this member is a decoy.

### Successful branch

```text
.heheh [gzip]
  -> TAR
  -> hidden_file
  -> (gzip -> bzip2) repeated 6 times
  -> gzip
  -> "ACM{too__eazyy_right}\n"
```

The successful route takes 19 archive/decompression operations from the input. The recovered payload is 22 bytes, including its trailing newline.

## Reproduce

Python 3 is sufficient; no third-party packages are needed. From this task folder, run:

```sh
python solve.py
```

To inspect another copy of the input:

```sh
python solve.py /path/to/txtpyc
```

Expected final output:

```text
TOKEN: ACM{too__eazyy_right}
```

The solver processes all regular members, including dotfiles, and prints each layer. It reads archive members in memory, ignores non-regular TAR members, and never executes the recovered contents or writes archive-controlled member paths. It applies depth and per-item size limits; it is intended for this small challenge, not unrestricted hostile archives.

## Included evidence

- `txtpyc`: analyzed input.
- `solve.py`: reproducible extraction script.
- `extraction-log.txt`: complete traversal of both branches.
- `payload.txt`: recovered token payload.

## Verification scope

The script was run successfully against the included input and recovered the token above. The input came from an existing `txtpyc` copy in Downloads. The authenticated challenge page lists an attachment with that name, but byte identity with the current server attachment was not independently verified. The token has not been submitted to CTFd for acceptance.

## Submission placement

Put these files under `questline-N/You_know_this/` in `ACMRecruitment-prabhasiva456`. Replace `N` with the assigned questline number and use the task folder name required by the organizers, if different. Neither was specified in the supplied instructions.

Submit the direct task-folder URL, not the repository root:

```text
https://github.com/YOUR_GITHUB_USERNAME/ACMRecruitment-prabhasiva456/tree/main/questline-N/You_know_this/
```
