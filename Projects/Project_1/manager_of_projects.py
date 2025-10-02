#==================================================================
def create():
    notes = input("Enter note text: ")
    with open("notes.txt", "a") as file:
        file.write(notes + "\n")
    print("Note created successfully.")

#==================================================================
def delete():
    with open("notes.txt", "r") as file:
        notes = file.readlines()
    if not notes:
        print("No notes to delete.")
        return
    
    print("\nNotes list:")
    for i, note in enumerate(notes, 1):
        print(f"{i}. {note.strip()}")
    numb = int(input("Enter note number to delete: "))
    if numb < 1 or numb > len(notes):
        print("Invalid note number.")
        return
    
    del notes[numb - 1]
    with open("notes.txt", "w") as file:
        for note in notes:
            file.write(note)
    print(f"Note #{numb} deleted successfully.")

#==================================================================
def search():
    with open("notes.txt", "r") as file:
        notes = file.readlines()
    if not notes:
        print("No notes to search.")
        return
    
    search = input("Enter text to search: ").lower()
    found = []
    for i, note in enumerate(notes, 1):
        if search in note.lower():
            found.append((i, note.strip()))
    if found:
        print(f"\nFound notes: {len(found)}")
        print("=" * 50)
        for number, text in found:
            print(f"#{number}: {text}")
        print("=" * 50)
    else:
        print("No notes found with this text.")

#==================================================================
def close():
    print("Ending session.")
    exit()

#==================================================================
def show():
    with open("notes.txt", "r") as file:
        notes = file.readlines()
    if not notes:
        print("No notes to show. You can create one.")
        return
    
    print(f"\nTotal notes: {len(notes)}")
    print("=" * 50)
    for i, note in enumerate(notes, 1):
        print(f"#{i}: {note.strip()}")
    print("=" * 50)

#==================================================================
def interface():
    print('''Welcome.''')
    while True:
        print('''What would you like to do?
        1 - Create note
        2 - Delete note
        3 - Search note
        4 - Close programm
        5 - Show notes
        Type the number for the answer.''')
        answer = input()
        match answer:
            case "1":
                create()
            case "2":
                delete()
            case "3":
                search()
            case "4":
                close()
            case "5":
                show()
            case _:
                print("This command does not exist. Try again.")
                continue

#==================================================================
interface()