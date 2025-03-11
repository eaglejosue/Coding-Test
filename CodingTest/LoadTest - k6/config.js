export default {
  BASE_URL: "http://localhost:5000", // ajuste para a URL base da sua API
  params: {
    headers: {
      "Content-Type": "application/json",
    },
  },
  optionsPost: {
    vus: 10,         // número de usuários virtuais para o teste POST
    duration: "30s", // duração do teste POST
  },
  optionsGet: {
    vus: 50,         // número de usuários virtuais para o teste GET
    duration: "30s", // duração do teste GET
  },
};
