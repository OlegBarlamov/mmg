import React, {PureComponent} from "react";
import {IBattleDefinition} from "../battle/IBattleDefinition";
import {IServiceLocator} from "../services/serviceLocator";

export interface IMenuComponentProps {
    serviceLocator: IServiceLocator
    onBattleSelected(definition: IBattleDefinition): void
}

interface IMenuComponentState {
    availableBattles: IBattleDefinition[] | null 
    isLoading: boolean
}

export class MenuComponent extends PureComponent<IMenuComponentProps, IMenuComponentState> {
    private retryTimeoutId: number | null = null;
    private readonly RETRY_DELAY_MS = 2000; // 2 seconds
    
    constructor(props: IMenuComponentProps) {
        super(props)
        
        this.state = {
            availableBattles: null,
            isLoading: true
        }
    }
    
    async componentDidMount() {
        await this.fetchBattles()
    }
    
    componentWillUnmount() {
        // Clear any pending retry timeout
        if (this.retryTimeoutId) {
            clearTimeout(this.retryTimeoutId)
            this.retryTimeoutId = null
        }
    }
    
    // Expose this method so parent can call it
    public async refreshBattles() {
        await this.fetchBattles()
    }
    
    private async fetchBattles() {
        this.setState({ isLoading: true })
        try {
            const battlesProvider = this.props.serviceLocator.battlesProvider()
            const battles = await battlesProvider.fetchBattles()
            this.setState({availableBattles: battles, isLoading: false})
        } catch (error) {
            console.error('Failed to fetch battles:', error)
            
            // Retry after delay
            this.retryTimeoutId = window.setTimeout(() => {
                this.fetchBattles()
            }, this.RETRY_DELAY_MS)
        }
    }
    
    render() {
        return (
            <div>
                <h1>Select a Battle</h1>
                {
                    this.state.isLoading ? (
                        <div>Loading...</div>
                    ) : this.state.availableBattles ? (
                        this.state.availableBattles.map((battle, index) => (
                            <button key={index} onClick={() => this.props.onBattleSelected(battle)}>
                                Size: {battle.width}x{battle.height}
                            </button>
                        ))
                    ) : (
                        <div>No battles available</div>
                    )
                }
            </div>
        )
    }
}