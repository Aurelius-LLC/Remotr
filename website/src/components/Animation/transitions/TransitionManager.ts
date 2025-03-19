import { useRef, useState, useCallback } from 'react';
import { Transition, TransitionState } from './types';
import { Step1To2Transition } from './Step1To2Transition';
import { Step2To3Transition } from './Step2To3Transition';
import { Step3To4Transition } from './Step3To4Transition';

const TRANSITIONS = [
  Step1To2Transition,
  Step2To3Transition,
  Step3To4Transition
];

export const useTransitionManager = (svgRef: React.RefObject<SVGSVGElement>) => {
  const [state, setState] = useState<TransitionState>({
    currentStep: 1,
    isAnimating: false,
    activeAnimations: []
  });

  const currentTransition = useRef<Transition | null>(null);

  const handleNext = useCallback(async () => {
    if (state.currentStep >= TRANSITIONS.length + 1 || state.isAnimating || !svgRef.current) {
      return;
    }

    setState(prev => ({ ...prev, isAnimating: true }));
    const transitionIndex = state.currentStep - 1;
    
    try {
      currentTransition.current = TRANSITIONS[transitionIndex];
      await currentTransition.current.forward(svgRef.current, false);
      setState(prev => ({
        ...prev,
        currentStep: prev.currentStep + 1,
        isAnimating: false
      }));
    } catch (error) {
      console.error('Error during forward transition:', error);
      setState(prev => ({ ...prev, isAnimating: false }));
    }
  }, [state.currentStep, state.isAnimating, svgRef]);

  const handleBack = useCallback(async () => {
    if (state.currentStep <= 1 || state.isAnimating || !svgRef.current) {
      return;
    }

    setState(prev => ({ ...prev, isAnimating: true }));
    const transitionIndex = state.currentStep - 2;
    
    try {
      currentTransition.current = TRANSITIONS[transitionIndex];
      await currentTransition.current.backward(svgRef.current, false);
      setState(prev => ({
        ...prev,
        currentStep: prev.currentStep - 1,
        isAnimating: false
      }));
    } catch (error) {
      console.error('Error during backward transition:', error);
      setState(prev => ({ ...prev, isAnimating: false }));
    }
  }, [state.currentStep, state.isAnimating, svgRef]);

  const skipToStep = useCallback(async (targetStep: number) => {
    if (
      targetStep < 1 || 
      targetStep > TRANSITIONS.length + 1 || 
      targetStep === state.currentStep || 
      state.isAnimating || 
      !svgRef.current
    ) {
      return;
    }

    setState(prev => ({ ...prev, isAnimating: true }));
    
    try {
      if (targetStep > state.currentStep) {
        // Move forward
        for (let step = state.currentStep; step < targetStep; step++) {
          currentTransition.current = TRANSITIONS[step - 1];
          await currentTransition.current.forward(svgRef.current, true);
        }
      } else {
        // Move backward
        for (let step = state.currentStep; step > targetStep; step--) {
          currentTransition.current = TRANSITIONS[step - 2];
          await currentTransition.current.backward(svgRef.current, true);
        }
      }

      setState(prev => ({
        ...prev,
        currentStep: targetStep,
        isAnimating: false
      }));
    } catch (error) {
      console.error('Error during skip transition:', error);
      setState(prev => ({ ...prev, isAnimating: false }));
    }
  }, [state.currentStep, state.isAnimating, svgRef]);

  return {
    currentStep: state.currentStep,
    isAnimating: state.isAnimating,
    handleNext,
    handleBack,
    skipToStep
  };
}; 