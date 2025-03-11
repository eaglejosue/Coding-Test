// Função para gerar número inteiro aleatório entre min e max (inclusive)
function getRandomIntInclusive(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

// Arrays com os nomes conforme informado
const firstNames = [
    "Leia", "Sadie", "Jose", "Sara", "Frank",
    "Dewey", "Tomas", "Joel", "Lukas", "Carlos"
];
const lastNames = [
    "Liberty", "Ray", "Harrison", "Ronan", "Drew",
    "Powell", "Larsen", "Chan", "Anderson", "Lane"
];

// Função que gera uma lista de clientes com a quantidade definida
const generateCustomers = (count) => {
    const customers = [];
    let idCounter = 1;
    for (let i = 0; i < count; i++) {
        const firstName = firstNames[getRandomIntInclusive(0, firstNames.length - 1)];
        const lastName = lastNames[getRandomIntInclusive(0, lastNames.length - 1)];
        const age = getRandomIntInclusive(18, 90);
        customers.push({
            id: idCounter,
            firstName: firstName,
            lastName: lastName,
            age: age
        });
        idCounter++;
    }
    return customers;
};

// Gera 50 clientes para o payload do POST
export default {
    PostCustomers: generateCustomers(50)
};
