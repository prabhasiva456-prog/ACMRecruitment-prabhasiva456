\# Buried\_escape — Forensics Write-up



\## Challenge



A suspicious file was recovered from a compromised server. The objective was to inspect it and recover a hidden flag.



\*\*Flag format:\*\* `acm{...}`



\## 1. Inspect the attachment



The challenge provided `release.zip`, containing:



```text

release/evidence.png

```



Despite its `.png` extension, the file begins with the JPEG signature:



```text

FF D8 FF E0

```



Opening the image displays “Server Room Access Log Snapshot,” but no visible flag.



\## 2. Locate the hidden archive



The JPEG end marker, `FF D9`, occurs at byte offset \*\*6550\*\*. Immediately after this marker, at offset \*\*6552\*\*, the file contains a ZIP signature:



```text

50 4B 03 04

```



The remaining \*\*1122 bytes\*\* form an appended ZIP archive containing a password-protected file named `flag.pdf`.



This explains why the image appears normal: the image viewer displays the JPEG and ignores the appended archive.



\## 3. Recover the ZIP password



I tested passwords locally against the embedded ZIP. Initial common-password lists did not find a match.



Using the RockYou wordlist recovered:



```text

flirt92

```



The password successfully decrypted `flag.pdf`, and the ZIP integrity check passed.



\## 4. Extract the flag



The recovered PDF is titled \*\*“Confidential - Internal Investigation Report.”\*\*



Under “Recovered artifact flag,” it contains:



```text

acm{f0rensics\_1s\_fun!!!}

```



I confirmed the flag through both PDF text extraction and visual inspection.



\## Reproduction commands



From the folder containing `release.zip` and the supplied `solve.py`, run in Git Bash:



```bash

python -m pip install pypdf

python solve.py release.zip --password flirt92 --output recovered

```



To reproduce the password search using a local RockYou wordlist:



```bash

python solve.py release.zip --wordlist /path/to/rockyou.txt --output recovered

```



The solver saves the JPEG, appended ZIP, and recovered PDF, then prints the PDF text when `pypdf` is installed.



\## Result



\*\*Flag:\*\* `acm{f0rensics\_1s\_fun!!!}`



The challenge combines a misleading file extension, an archive appended after an image’s end marker, and ZIP password protection.



The flag was recovered from the downloaded attachment; it has not been submitted to CTFd for acceptance.

