import axios from "axios";

const agent = axios.create({
    baseURL: import.meta.env.VITE_API_URL
});

const sleep = (delay: number) => {
    return new Promise(resolve => {
        setTimeout(resolve, delay)
    });
}

agent.interceptors.response.use(async response => {
    try {
        await sleep(1000);
        return response;
    } catch (error) {
        console.log(error);
        return Promise.reject(error);
    }
})

export default agent;