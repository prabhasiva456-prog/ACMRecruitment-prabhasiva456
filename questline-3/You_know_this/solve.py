"""Inspect every archive branch without executing files or trusting member paths."""
from pathlib import Path
import argparse
import bz2
import gzip
import io
import lzma
import re
import tarfile
import zipfile

MAX_SIZE = 32 * 1024 * 1024

def walk(data, route, depth=0):
    if depth > 64 or len(data) > MAX_SIZE:
        raise ValueError('Inspection limit exceeded')
    print(f'{depth:02d} | {len(data):6d} bytes | {route}')
    for signature, opener, label in (
        (b'BZh', bz2.BZ2File, 'bzip2'),
        (b'\x1f\x8b', gzip.open, 'gzip'),
        (b'\xfd7zXZ\x00', lzma.LZMAFile, 'xz'),
    ):
        if data.startswith(signature):
            with opener(io.BytesIO(data)) as stream:
                unpacked = stream.read(MAX_SIZE + 1)
            yield from walk(unpacked, route + ' -> ' + label, depth + 1)
            return
    if zipfile.is_zipfile(io.BytesIO(data)):
        with zipfile.ZipFile(io.BytesIO(data)) as archive:
            for member in archive.infolist():
                if member.is_dir():
                    continue
                if member.file_size > MAX_SIZE:
                    raise ValueError('Oversized ZIP member')
                yield from walk(archive.read(member), route + ' -> ZIP:' + member.filename, depth + 1)
        return
    if tarfile.is_tarfile(io.BytesIO(data)):
        with tarfile.open(fileobj=io.BytesIO(data)) as archive:
            for member in archive:
                if not member.isfile():
                    continue
                if member.size > MAX_SIZE:
                    raise ValueError('Oversized TAR member')
                with archive.extractfile(member) as stream:
                    yield from walk(stream.read(MAX_SIZE + 1), route + ' -> TAR:' + member.name, depth + 1)
        return
    try:
        text = data.decode('ascii')
    except UnicodeDecodeError:
        text = ''
    if text and re.fullmatch(r'[0-9a-fA-F\s]+', text):
        yield from walk(bytes.fromhex(text), route + ' -> hex', depth + 1)
        return
    print('LEAF:', repr(data))
    yield data

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('input', nargs='?', type=Path, default=Path(__file__).with_name('txtpyc'))
    args = parser.parse_args()
    flags = set()
    for leaf in walk(args.input.read_bytes(), args.input.name):
        flags.update(re.findall(rb'ACM\{[^}\r\n]+\}', leaf))
    for flag in sorted(flags):
        print('TOKEN:', flag.decode('ascii'))
    if not flags:
        raise SystemExit('No ACM token found')

