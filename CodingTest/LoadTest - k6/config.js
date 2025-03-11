export default {
  BASE_URL: "http://localhost:5000", // ajuste para a URL base da sua API
  params: {
    headers: {
      "Content-Type": "application/json",
    },
  },
  optionsPost: {
    vus: 10,         // n�mero de usu�rios virtuais para o teste POST
    duration: "30s", // dura��o do teste POST
  },
  optionsGet: {
    vus: 50,         // n�mero de usu�rios virtuais para o teste GET
    duration: "30s", // dura��o do teste GET
  },
};
