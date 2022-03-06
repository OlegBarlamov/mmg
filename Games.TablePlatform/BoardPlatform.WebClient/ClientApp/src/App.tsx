import React from 'react';
import './App.css';
import CanvasViewComponent from "./canvas/CanvasViewComponent";
import {FakeCanvasService} from "./canvas/FakeCanvasService";

function App() {
    initializeApplication()
    
    const canvasService = new FakeCanvasService()
    
    return (
        <div className="App">
            <CanvasViewComponent canvasService={canvasService} devicePixelRatio={window.devicePixelRatio} />
        </div>
    );
}

function initializeApplication() {
    
}

export default App;
