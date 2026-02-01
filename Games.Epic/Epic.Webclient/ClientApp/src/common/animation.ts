import * as PIXI from "pixi.js";

export interface AnimationOptions {
    duration: number; // in milliseconds
    easing?: 'linear' | 'easeInOut' | 'easeIn' | 'easeOut';
}

export interface AttackAnimationOptions extends AnimationOptions {
    attackDistance?: number; // How far the unit moves toward target (as percentage of distance)
    returnDuration?: number; // Duration for returning to original position
}

export class Animation {
    private static readonly defaultOptions: AnimationOptions = {
        duration: 500,
        easing: 'easeInOut'
    };

    private static readonly defaultAttackOptions: AttackAnimationOptions = {
        duration: 300,
        easing: 'easeInOut',
        attackDistance: 0.3, // Move 30% toward target
        returnDuration: 200
    };

    static async animatePosition(
        container: PIXI.Container,
        targetX: number,
        targetY: number,
        options: Partial<AnimationOptions> = {}
    ): Promise<void> {
        const opts = { ...this.defaultOptions, ...options };
        const startX = container.x;
        const startY = container.y;
        const deltaX = targetX - startX;
        const deltaY = targetY - startY;
        
        const startTime = Date.now();
        
        return new Promise((resolve) => {
            const animate = () => {
                const elapsed = Date.now() - startTime;
                const progress = Math.min(elapsed / opts.duration, 1);
                
                const easedProgress = this.applyEasing(progress, opts.easing!);
                
                container.x = startX + (deltaX * easedProgress);
                container.y = startY + (deltaY * easedProgress);
                
                if (progress < 1) {
                    requestAnimationFrame(animate);
                } else {
                    // Ensure final position is exact
                    container.x = targetX;
                    container.y = targetY;
                    resolve();
                }
            };
            
            animate();
        });
    }

    static async animateAttack(
        attackerContainer: PIXI.Container,
        targetContainer: PIXI.Container,
        options: Partial<AttackAnimationOptions> = {}
    ): Promise<void> {
        const opts = { ...this.defaultAttackOptions, ...options };
        
        const startX = attackerContainer.x;
        const startY = attackerContainer.y;
        
        // Calculate attack position (move toward target)
        const deltaX = targetContainer.x - startX;
        const deltaY = targetContainer.y - startY;
        const attackX = startX + (deltaX * opts.attackDistance!);
        const attackY = startY + (deltaY * opts.attackDistance!);
        
        // Move toward target
        await this.animatePosition(attackerContainer, attackX, attackY, {
            duration: opts.duration,
            easing: opts.easing
        });
        
        // Return to original position
        await this.animatePosition(attackerContainer, startX, startY, {
            duration: opts.returnDuration!,
            easing: opts.easing
        });
    }

    static async animateDamage(
        targetContainer: PIXI.Container,
        options: Partial<AnimationOptions> = {}
    ): Promise<void> {
        const opts = { ...this.defaultOptions, ...options };
        const originalScale = targetContainer.scale.x;
        
        const startTime = Date.now();
        
        return new Promise((resolve) => {
            const animate = () => {
                const elapsed = Date.now() - startTime;
                const progress = Math.min(elapsed / opts.duration, 1);
                
                // Create a "shake" effect with scale and rotation
                const shakeIntensity = 1 - progress; // Shake less as animation progresses
                const scale = originalScale + (shakeIntensity * 0.1);
                const rotation = Math.sin(progress * Math.PI * 8) * shakeIntensity * 0.1; // 8 oscillations
                
                targetContainer.scale.set(scale);
                targetContainer.rotation = rotation;
                
                if (progress < 1) {
                    requestAnimationFrame(animate);
                } else {
                    // Restore original state
                    targetContainer.scale.set(originalScale);
                    targetContainer.rotation = 0;
                    resolve();
                }
            };
            
            animate();
        });
    }

    static async animateWait(
        container: PIXI.Container,
        options: Partial<AnimationOptions> = {}
    ): Promise<void> {
        const opts = { ...this.defaultOptions, ...options };
        const originalAlpha = container.alpha;
        
        const startTime = Date.now();
        
        return new Promise((resolve) => {
            const animate = () => {
                const elapsed = Date.now() - startTime;
                const progress = Math.min(elapsed / opts.duration, 1);
                
                // Create a gentle pulse effect with alpha
                const pulseIntensity = 0.3; // How much the alpha changes
                const pulseFrequency = 2; // How many pulses per animation
                const alpha = originalAlpha + (pulseIntensity * Math.sin(progress * Math.PI * pulseFrequency));
                
                container.alpha = Math.max(0.3, Math.min(1.0, alpha)); // Clamp between 0.3 and 1.0
                
                if (progress < 1) {
                    requestAnimationFrame(animate);
                } else {
                    // Restore original alpha
                    container.alpha = originalAlpha;
                    resolve();
                }
            };
            
            animate();
        });
    }

    static async animateHeal(
        container: PIXI.Container,
        options: Partial<AnimationOptions> = {}
    ): Promise<void> {
        const opts = { ...this.defaultOptions, ...options };
        const originalScale = container.scale.x;
        
        const startTime = Date.now();
        
        return new Promise((resolve) => {
            const animate = () => {
                const elapsed = Date.now() - startTime;
                const progress = Math.min(elapsed / opts.duration, 1);
                
                // Create a "grow" effect - scale up slightly then back down
                const growIntensity = Math.sin(progress * Math.PI); // Smooth grow and shrink
                const scale = originalScale + (growIntensity * 0.15);
                
                container.scale.set(scale);
                
                if (progress < 1) {
                    requestAnimationFrame(animate);
                } else {
                    // Restore original scale
                    container.scale.set(originalScale);
                    resolve();
                }
            };
            
            animate();
        });
    }

    private static applyEasing(progress: number, easing: string): number {
        switch (easing) {
            case 'linear':
                return progress;
            case 'easeInOut':
                return progress < 0.5 
                    ? 2 * progress * progress 
                    : 1 - Math.pow(-2 * progress + 2, 2) / 2;
            case 'easeIn':
                return progress * progress;
            case 'easeOut':
                return 1 - Math.pow(1 - progress, 2);
            default:
                return progress;
        }
    }
}
