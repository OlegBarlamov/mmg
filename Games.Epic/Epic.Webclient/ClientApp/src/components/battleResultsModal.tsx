import React, { PureComponent } from 'react';
import './battleResultsModal.css';
import { IReportInfo, IReportUnit } from '../services/serverAPI';

export interface IBattleResultsModalProps {
    report: IReportInfo;
    onOk: () => void;
}

export class BattleResultsModal extends PureComponent<IBattleResultsModalProps> {
    render() {
        const { report } = this.props;
        
        return (
            <div className="battle-results-modal-overlay">
                <div className="battle-results-modal">
                    <div className="battle-results-header">
                        <h2>Battle Results</h2>
                        <div className={`battle-result ${report.isWinner ? 'victory' : 'defeat'}`}>
                            {report.isWinner ? 'Victory!' : 'Defeat'}
                        </div>
                    </div>
                    
                    <div className="battle-results-content">
                        <div className="player-losses">
                            <h3>Your Losses</h3>
                            <div className="units-horizontal">
                                {report.playerUnits
                                    .filter(unit => unit.startCount > unit.finalCount)
                                    .map((unit, index) => (
                                    <div key={index} className="unit-horizontal-item">
                                        <img 
                                            src={unit.thumbnailUrl} 
                                            alt={unit.name} 
                                            className="unit-thumbnail"
                                        />
                                        <span className="unit-losses-count">-{unit.startCount - unit.finalCount}</span>
                                    </div>
                                ))}
                                {report.playerUnits.filter(unit => unit.startCount > unit.finalCount).length === 0 && (
                                    <div className="no-losses">No units lost</div>
                                )}
                            </div>
                        </div>
                        
                        <div className="enemy-losses">
                            <h3>Enemy Losses</h3>
                            <div className="units-horizontal">
                                {report.enemyUnits
                                    .filter(unit => unit.startCount > unit.finalCount)
                                    .map((unit, index) => (
                                    <div key={index} className="unit-horizontal-item">
                                        <img 
                                            src={unit.thumbnailUrl} 
                                            alt={unit.name} 
                                            className="unit-thumbnail"
                                        />
                                        <span className="unit-losses-count">-{unit.startCount - unit.finalCount}</span>
                                    </div>
                                ))}
                                {report.enemyUnits.filter(unit => unit.startCount > unit.finalCount).length === 0 && (
                                    <div className="no-losses">No units lost</div>
                                )}
                            </div>
                        </div>
                    </div>
                    
                    <div className="battle-results-footer">
                        <button 
                            className="ok-button"
                            onClick={this.props.onOk}
                        >
                            OK
                        </button>
                    </div>
                </div>
            </div>
        );
    }
} 