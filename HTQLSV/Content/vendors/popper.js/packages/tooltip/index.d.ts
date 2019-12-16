
export type TitleFunction = () => string;

export type Delay = Record<'show' | 'hide', number>;

export interface Options {
    container?: HTMLElement | string;
    delay?: number | Delay;
    html?: boolean;
    template?: string;
    title?: string | HTMLElement | TitleFunction;
    /**
     * available options are click, hover, focus, manual
     * required to form a space delimited string
     * e.g. 'hover focus'
     */
    trigger?: string;
    offset?: number | string;
}

declare class Tooltip {
    constructor(reference: HTMLElement, options: Options);

    _isOpen: boolean;

    show(): void;

    hide(): void;

    dispose(): void;

    toggle(): void;
}

export default Tooltip;
