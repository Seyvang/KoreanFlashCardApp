import csv
import re
from pathlib import Path


ROOT = Path(__file__).resolve().parent
CSV_PATH = ROOT / "Topik1000to2000SentenceMerged.csv"
CS_PATH = ROOT.parent / "KoreanFlashCardApp" / "Data" / "KoreanWordSentences.cs"


def csharp_string(value: str) -> str:
    escaped = []
    for char in value:
        if char == "\\":
            escaped.append("\\\\")
        elif char == '"':
            escaped.append('\\"')
        elif char == "\n":
            escaped.append("\\n")
        elif char == "\r":
            escaped.append("\\r")
        elif char == "\t":
            escaped.append("\\t")
        else:
            escaped.append(char)
    return '"' + "".join(escaped) + '"'


def load_sentences() -> dict[int, tuple[str, str]]:
    rows: dict[int, tuple[str, str]] = {}
    with CSV_PATH.open("r", encoding="utf-8-sig", newline="") as file:
        reader = csv.DictReader(file)
        for row in reader:
            number = int(row["Number"])
            rows[number] = (
                row["ExampleSentence"].strip(),
                row["ExampleSentenceTranslation"].strip(),
            )
    return rows


def split_args(block: str) -> list[tuple[int, int, str]]:
    args: list[tuple[int, int, str]] = []
    start = block.index("(") + 1
    end = block.rindex(")")
    arg_start = start
    in_string = False
    escaped = False

    for index in range(start, end):
        char = block[index]
        if in_string:
            if escaped:
                escaped = False
            elif char == "\\":
                escaped = True
            elif char == '"':
                in_string = False
        elif char == '"':
            in_string = True
        elif char == ",":
            args.append((arg_start, index, block[arg_start:index]))
            arg_start = index + 1

    args.append((arg_start, end, block[arg_start:end]))
    return args


def replace_arg(block: str, arg: tuple[int, int, str], value: str) -> str:
    start, end, text = arg
    indent = re.match(r"\s*", text).group(0)
    return block[:start] + indent + value + block[end:]


def update_block(block: str, rows: dict[int, tuple[str, str]]) -> tuple[str, bool]:
    args = split_args(block)
    if len(args) < 6:
        return block, False

    word_id = int(args[1][2].strip())
    if word_id not in rows:
        return block, False

    sentence, translation = rows[word_id]
    updated = replace_arg(block, args[5], csharp_string(translation))
    updated_args = split_args(updated)
    updated = replace_arg(updated, updated_args[3], csharp_string(sentence))
    return updated, True


def main() -> None:
    rows = load_sentences()
    content = CS_PATH.read_text(encoding="utf-8")
    pattern = re.compile(r"new WordSentenceImport\([\s\S]*?\),")

    count = 0

    def repl(match: re.Match[str]) -> str:
        nonlocal count
        updated, changed = update_block(match.group(0), rows)
        if changed:
            count += 1
        return updated

    updated = pattern.sub(repl, content)
    CS_PATH.write_text(updated, encoding="utf-8")
    print(f"Updated {count} WordSentenceImport entries in {CS_PATH}")


if __name__ == "__main__":
    main()
