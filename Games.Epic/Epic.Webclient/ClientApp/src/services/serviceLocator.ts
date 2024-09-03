import {CanvasService, ICanvasService} from "./canvasService";
import {BattleMapService, IBattleMapService} from "./battleMapService";

export interface IServiceLocator {
    canvasService() : ICanvasService
    battleMapService() : IBattleMapService
}

export class ExplicitServiceLocator implements IServiceLocator {
    private singletons = new Map<string, object>()
    public getSingletoneService<T extends object>(serviceName: string, factory: () => T) : T  {
        if (this.singletons.has(serviceName)) {
            return this.singletons.get(serviceName) as T;
        }

        const instance = factory()
        this.singletons.set(serviceName, instance)
        return instance
    }
    
    canvasService(): ICanvasService {
        return this.getSingletoneService("CanvasService", () => new CanvasService())
    }

    battleMapService(): IBattleMapService {
        return this.getSingletoneService("BattleMapService", () => new BattleMapService(
            this.canvasService(),
        ))
    }
}