import csv
import sys

def get_word_type(korean_word):
    if korean_word.endswith('하다'):
        return 8 # Verb
    elif korean_word.endswith('다'):
        return 8 # Verb
    elif korean_word in ['만약', '점점', '너무나', '막', '무척', '원래', '그리하여', '일찍', '다소', '가득', '그저', '충분히', '미리', '오랜', '적극적', '점차', '앞서', '당장', '스스로', '어쩌면', '아까', '자꾸', '그리', '게다가', '오랫동안', '혹시', '적어도', '분명히', '어쨌든', '널리', '마침내', '어서', '그만큼', '비로소', '아예', '어찌', '금방', '상당히', '한참', '참으로', '겨우', '비교적', '천천히', '대체로', '몹시', '문득', '가까이', '언젠가', '가만히', '조금씩', '만일', '단순히', '보통', '참', '막', '끊임없이', '차라리', '사실상', '깨끗이', '아무래도', '가령', '이하', '아마도', '오직', '잠시', '덜', '본래', '잔뜩', '최소한', '굉장히', '살짝', '여기저기', '아울러', '서서히', '어제', '대단히', '마주', '온통', '자세히', '정확히', '마침', '한꺼번에']:
        return 5 # Adverb
    else:
        return 1 # Noun

with open('D:\\Code\\KoreanFlashCardApp\\words\\Topik2000Word.csv', 'r', encoding='cp949') as infile, open('D:\\Code\\KoreanFlashCardApp\\words\\Topik2000Word_processed.csv', 'w', encoding='utf-8', newline='') as outfile:
    reader = csv.reader(infile)
    writer = csv.writer(outfile)
    header = next(reader) # Skip header
    writer.writerow(header)
    for row in reader:
        korean_word = row[1].strip()
        word_type = get_word_type(korean_word)
        row[3] = word_type
        writer.writerow(row)

print('Processing complete. The new file is saved as Topik2000Word_processed.csv')