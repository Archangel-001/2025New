def create():
    notes = input("Enter note text: ")
    with open("notes.txt", "a") as file:
        file.write(notes + "\n")
    print("Note created successfully.")

def delete():
    pass

def search():
    pass

def close():
    with open("notes.txt", "r") as file:
        notes = file.readlines()
    if not notes:
        print("No notes to close.")
        return
    
    print("\nList of notes:")
    for i, note in enumerate(notes, 1):
        print(f"{i}. {note.strip()}")
    numb = int(input("Enter note number to close: "))
    if numb < 1 or numb > len(notes):
        print("Invalid note number!")
        return
    print(f"Note #{numb} has been closed.")

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

def interface():
    print('''Welcome.''')
    while True:
        print('''What would you like to do?
        1 - Create note
        2 - Delete note
        3 - Search note
        4 - Close note
        5 - Show note
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
            