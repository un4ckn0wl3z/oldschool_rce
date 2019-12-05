import hashlib

PREFIX = 'iPA'
CONSTANT_SALT = 'Z'


def insert_salt(target_string, index):
    return target_string[:index] + ' ' + target_string[index:]


name = input('Enter your name: ')
md5_name = hashlib.md5(name.encode())
print("Your hashed: ", md5_name.hexdigest())

salted = PREFIX+str(insert_salt(insert_salt(md5_name.hexdigest(), 2), 4).replace(' ', 'Z')).upper()


print("Your key: ",  hashlib.md5(salted.encode()).hexdigest())



