// get-customers-test.js
import { check, sleep } from "k6";
import http from "k6/http";
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

import config from "./config.js";

export let options = config.optionsGet;

export function handleSummary(data) {
    return {
        "summary.html": htmlReport(data),
    };
}

export default function () {
    // Cria um CacheKey dinâmico
    const url = `${config.BASE_URL}/api/customer?CacheKey=loadTest_${__VU}_${__ITER}`;
    const response = http.get(url, config.params);

    check(response, {
        "response code was 200": (r) => r.status === 200,
        "response code was 204": (r) => r.status === 204,
    });

    sleep(1);
}
