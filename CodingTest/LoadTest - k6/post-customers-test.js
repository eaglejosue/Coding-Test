import { check, sleep } from "k6";
import http from "k6/http";
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

import config from "./config.js";
import payload from "./payload.js";

export let options = config.optionsPost;

export function handleSummary(data) {
    return {
        "summary.html": htmlReport(data),
    };
}

export default function () {
    const url = `${config.BASE_URL}/api/customer`;

    // Envia o payload (lista de 50 clientes) como JSON
    const response = http.post(url, JSON.stringify(payload.PostCustomers), config.params);

    // Verifica se o status retornado é 200 (OK) ou 400 (BadRequest)
    check(response, {
        "response code was 200": (r) => r.status === 200,
        "response code was 400": (r) => r.status === 400,
    });

    if (response.status !== 200 && response.status !== 400) {
        console.log("Unexpected response: ", response.body);
    }

    sleep(1);
}
