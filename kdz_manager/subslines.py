import re
import pdb

def get_comment(txt):
    m = re.search('(///.+)', txt)
    if m:
        found = m.group(1)
    return found

def process_file(fname, linenum, subs):
    linenum -= 1
    with open(fname, "r+") as f:
        lines = f.readlines()
        old = lines[linenum]
        print("old:__" + old)
        #pdb.set_trace()
        # get first comment marker
        comment = old.index('/')
        new = old[0:comment] + subs + '\n'
        print("new:__" + new)
        #pdb.set_trace()
        lines[linenum] = new
        f.seek(0)
        f.writelines(lines)
        f.truncate()

with open('rus_strings.txt', 'r') as inputfile:
    for line in inputfile:
        data = line.split(':')
        fname = data[0].strip()
        lnum = int(data[1].strip())
        newtxt = get_comment(data[2])
        process_file(fname, lnum, newtxt)

