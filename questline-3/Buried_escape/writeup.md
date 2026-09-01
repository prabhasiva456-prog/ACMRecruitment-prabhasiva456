# Buried Escape

## Problem Description

A suspicious image file was recovered from a compromised server. Although the image appeared normal, the objective was to inspect the file more deeply and recover the hidden flag.

## Approach

I first extracted `release.zip`, which contained a file named `evidence.png`. Although it had a `.png` extension, its magic bytes were `FF D8 FF E0`, showing that it was actually a JPEG file.

The image displayed the text "Server Room Access Log Snapshot," but no flag was visible. I then inspected the raw bytes and found the JPEG end marker `FF D9` at byte offset 6550. Immediately after it, at offset 6552, was the ZIP signature `50 4B 03 04`.

This revealed that a ZIP archive had been appended to the end of the JPEG. The hidden archive contained a password-protected file named `flag.pdf`. I tested the archive with the RockYou password list and recovered the password:

`flirt92`

After extracting and opening the PDF, I found the hidden flag.

## Final Answer

`acm{f0rensics_1s_fun!!!}`
