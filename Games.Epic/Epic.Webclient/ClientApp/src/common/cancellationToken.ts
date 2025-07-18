import { Signal } from "typed-signals"

export class TaskCancelledError extends Error {
    constructor() {
        super("Task cancelled")
    }
}

export interface CancellationToken {
    onCancel(cancellationHandler: () => void): void
}

export interface ICancellationTokenSource {
    get token(): CancellationToken
    get isCanceled(): boolean
    
    cancel(): void
    dispose(): void
}

export class SignalBasedCancellationToken implements ICancellationTokenSource, CancellationToken {
    isCanceled: boolean = false
    private cancellationHandler: (() => void) | null = null

    constructor(private readonly signal: Signal<() => void>) {
        this.signal.connect(this.cancel.bind(this))
    }
    onCancel(cancellationHandler: () => void): void {
        if (this.isCanceled) {
            cancellationHandler()
        } else {
            this.cancellationHandler = cancellationHandler
        }
    }
    get token(): CancellationToken {
        return this
    }
    cancel(): void {
        if (this.isCanceled) return
        this.isCanceled = true
        if (this.cancellationHandler) {
            this.cancellationHandler()
        }
    }
    dispose(): void {
        this.cancellationHandler = null
        this.signal.disconnect(this.cancel.bind(this))
    }
}