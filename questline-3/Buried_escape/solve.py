"""Recover the password-protected PDF appended to the challenge JPEG."""
import argparse
import io
from pathlib import Path
import zipfile
import zlib

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('archive', nargs='?', type=Path, default=Path(__file__).with_name('release.zip'))
parser.add_argument('--password', help='Password for the embedded ZIP')
parser.add_argument('--wordlist', type=Path, help='Optional newline-separated password list')
parser.add_argument('--output', type=Path, default=Path('recovered'))
args = parser.parse_args()
with zipfile.ZipFile(args.archive) as outer:
    image = outer.read('release/evidence.png')
assert image.startswith(b'\xff\xd8\xff'), 'Expected JPEG signature'
end = image.find(b'\xff\xd9') + 2
assert end > 1 and image[end:end+4] == b'PK\x03\x04', 'Expected ZIP after JPEG end'
print(f'JPEG ends at offset {end}; appended ZIP is {len(image)-end} bytes')
embedded = zipfile.ZipFile(io.BytesIO(image[end:]))
if args.password:
    password = args.password.encode()
    payload = embedded.read('flag.pdf', pwd=password)
elif args.wordlist:
    payload = None
    with args.wordlist.open('rb') as words:
        for line in words:
            password = line.rstrip(b'\r\n')
            if not password:
                continue
            try:
                payload = embedded.read('flag.pdf', pwd=password)
            except (RuntimeError, zipfile.BadZipFile, zlib.error):
                continue
            print('ZIP password:', password.decode('utf-8', errors='replace'))
            break
    if payload is None:
        raise SystemExit('Password not found in supplied wordlist')
else:
    raise SystemExit('Provide --password or --wordlist')
args.output.mkdir(parents=True, exist_ok=True)
(args.output / 'evidence.jpg').write_bytes(image[:end])
(args.output / 'hidden.zip').write_bytes(image[end:])
(args.output / 'flag.pdf').write_bytes(payload)
print('Recovered:', args.output / 'flag.pdf')
try:
    from pypdf import PdfReader
except ImportError:
    print('Install pypdf to extract PDF text, or open the recovered PDF.')
else:
    for page in PdfReader(io.BytesIO(payload)).pages:
        print(page.extract_text())
