export interface IBattlePanelActionsController { 
    onWaitPressed: (() => void) | null
    onPassPressed: (() => void) | null
    onRansomPressed: (() => void) | null
}
