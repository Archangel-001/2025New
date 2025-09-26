def create():
    pass

def delete():
    pass

def search():
    pass

def close():
    pass

def show():
    pass

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