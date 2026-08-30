# Password Protected — Writeup

## Flag
ACM{selection_is_a_hectic_process}

## Challenge
Analyze a compiled executable, bypass its password gate, and recover the hidden flag.

## Investigation
Static analysis identified the file as a 64-bit Linux ELF executable.

Inspecting its strings revealed the password:
YouAreTooLazy

The program compares the supplied password using strcmp.
When the password matches, it decodes an embedded string by
XORing each byte with 7, then prints the result.

## Encoded String
FDJ|tbkbdsnhiXntXfXobdsndXwuhdbttz

## Reproduce the Decoding
Use Python:

    encoded = "FDJ|tbkbdsnhiXntXfXobdsndXwuhdbttz"
    print("".join(chr(ord(c) ^ 7) for c in encoded))

Output:
ACM{selection_is_a_hectic_process}

## Conclusion
The password was stored in plaintext, and the flag used simple XOR
obfuscation. Reconstructing the decoding logic recovered the flag
without executing the binary or passing through the password gate.
