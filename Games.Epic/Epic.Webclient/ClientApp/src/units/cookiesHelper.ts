import { getCookie, setCookie } from "typescript-cookie";

const sessionTokenCookieName = 'token'
export function getSessionCookie(): string | undefined {
    return getCookie(sessionTokenCookieName)
}

export function setSessionCookie(token: string): void {
    setCookie(sessionTokenCookieName, token)
}