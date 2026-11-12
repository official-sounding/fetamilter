const modes = ['auto', 'light', 'dark'] as const;
type ThemeMode = typeof modes[number];

 function parseThemeMode(modeStr: string | null): ThemeMode
    {
        if(modeStr && modes.some(m => m === modeStr.toLowerCase())) {
            return modeStr as unknown as ThemeMode;
        }

        return "auto";
    }

export class ThemeToggle extends HTMLElement {
    private html = document.querySelector('html')!;
    static modes = ['auto','light','dark'];

    constructor() {
        super();
        const ls = parseThemeMode(localStorage.getItem('theme-mode'));
        this.setMode(ls);
        const shadow = this.attachShadow({ mode: "open" });

        const select = document.createElement("select");
        for(let t of modes) {
            const el = document.createElement('option');
            el.value = t;
            el.innerText = t;
            if(t === ls) {
                el.selected = true;
            }
            select.appendChild(el);
        }

        select.addEventListener('change', (e: Event) => {
            if(e.target instanceof HTMLSelectElement) {
                this.setMode(parseThemeMode(e.target.value));
            }
        });
        shadow.appendChild(select);
    }

    setMode(mode: ThemeMode) {
        localStorage.setItem("theme-mode", mode);
        switch(mode) {
            case 'dark':
                this.html.style.setProperty('color-scheme', 'dark');
                document.documentElement.style.setProperty('--system-cs-is-light', '0');
                break;
            case 'light':
                this.html.style.setProperty('color-scheme', 'light');
                document.documentElement.style.setProperty('--system-cs-is-light', '1');
                break;
            case 'auto':
            default:
                const isLightMode = window.matchMedia && window.matchMedia('(prefers-color-scheme: light)').matches;
                this.html.style.setProperty('color-scheme', 'light dark');
                document.documentElement.style.setProperty('--system-cs-is-light', isLightMode ? '1' : '0');
                break;
        }
    }
}
