#!/usr/bin/env python3
"""
Build the updated Topik CSV by:
1. Reading the original CSV
2. Looking up example sentences from a sentences file
3. Writing the combined output
"""
import csv, os

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
ORIGINAL = os.path.join(SCRIPT_DIR, "Topik1000to2000processed_backup.csv")
SENTENCES = os.path.join(SCRIPT_DIR, "sentences.csv")
OUTPUT = os.path.join(SCRIPT_DIR, "Topik1000to2000SentenceMerged.csv")

# Step 1: Load sentences keyed by Number
sents = {}
with open(SENTENCES, "r", encoding="utf-8") as f:
    reader = csv.reader(f)
    for row in reader:
        if len(row) >= 2:
            sents[row[0].strip()] = (row[1].strip(), row[2].strip() if len(row) >= 3 else "")

# Step 2: Read original, append sentences, write output
with open(ORIGINAL, "r", encoding="utf-8") as fin, \
     open(OUTPUT, "w", encoding="utf-8", newline="") as fout:
    reader = csv.reader(fin)
    writer = csv.writer(fout)

    header = next(reader)
    # Remove trailing empty columns
    while header and header[-1].strip() == "":
        header.pop()
    header.extend(["ExampleSentence", "ExampleSentenceTranslation"])
    writer.writerow(header)

    count = 0
    for row in reader:
        # Remove trailing empty columns
        while row and row[-1].strip() == "":
            row.pop()
        num = row[0].strip()
        if num in sents:
            row.extend([sents[num][0], sents[num][1]])
        else:
            row.extend(["", ""])
        writer.writerow(row)
        count += 1

print(f"Wrote {count} data rows to {OUTPUT}")
