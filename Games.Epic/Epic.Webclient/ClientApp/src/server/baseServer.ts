const basicHeaders = {
    "Content-Type": "application/json"
}

export class BaseServer {
    private readonly baseServerURL: URL
    constructor(private readonly baseUrl: string) {
        this.baseServerURL = new URL(baseUrl)
    }

    protected async fetchResource<T>(route: string, method: "GET" | "POST", resourceName: string, body?: any): Promise<T> {
        const response = await fetch(`${this.baseUrl}/${route}`, {
            method: method,
            headers: basicHeaders,
            body: body ? JSON.stringify(body) : undefined,
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || `Failed to fetch ${resourceName}.`);
        }

        const contentType = response.headers.get("content-type") || "";
        if (contentType.includes("application/json")) {
            return await response.json();
        } else {
            return await response.text() as T;
        }
    }
    
    protected establishWS(route: string): Promise<WebSocket> {
        const protocol = this.baseServerURL.protocol === "https:" ? "wss" : "ws"
        const wsUrl = `${protocol}://${this.baseServerURL.host}/${route}/ws`
        const webSocket = new WebSocket(wsUrl)
        
        return new Promise((resolve, reject) => {
            webSocket.onopen = () => {
                webSocket.onopen = null
                webSocket.onerror = null
                resolve(webSocket)
            }
            webSocket.onerror = (e) => {
                webSocket.onopen = null
                webSocket.onerror = null
                reject(e)
            }
        })
    }
}