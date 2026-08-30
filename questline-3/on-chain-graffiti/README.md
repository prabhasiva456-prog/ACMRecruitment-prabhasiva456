# On-Chain Graffiti Writeup

## Flag
CTF{0n_ch41n_gr4ff1t1_1sn7_pr1v4t3}

## Solution
1. Opened transactions.json from BLOCKCHAIN-CHALLENGE.zip.
2. Looked for zero-value transactions carrying input data.
3. Found a suspicious transaction at block 1042917 sending 0 ETH to:
   0x000000000000000000000000000000000000dEaD
4. Removed the 0x prefix from its input and decoded the hex as UTF-8.

Transaction hash:
0xfe22a4248ac9ed336de7daecd3ada8b4f2222d3b41a3dbd199b364f73bb387d0

Encoded input:
0x67672065617379206875683f204354467b306e5f636834316e5f67723466663174315f31736e375f707231763474337d

Decoded message:
gg easy huh? CTF{0n_ch41n_gr4ff1t1_1sn7_pr1v4t3}

## Conclusion
The attacker hid the flag in transaction input data.
A transaction can carry a message even when it transfers no ETH.
