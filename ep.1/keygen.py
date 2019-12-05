import hashlib


def insert_salted(string, index):
    return string[:index] + ' ' + string[index:]


name = input("Enter Name: ")
md5_name = hashlib.md5(name.encode())
print("Your hashed: ", md5_name.hexdigest())

salted = str(insert_salted(insert_salted(md5_name.hexdigest(), 2), 4)).replace(' ', 'Z').upper()
salted = 'iPA' + salted
print("Your serial: ", hashlib.md5(salted.encode()).hexdigest())
