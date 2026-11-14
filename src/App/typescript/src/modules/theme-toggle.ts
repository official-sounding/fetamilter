const modes = ['auto', 'light', 'dark'] as const;
type ThemeMode = (typeof modes)[number];

function parseThemeMode(modeStr: string | null): ThemeMode {
    if (modeStr && modes.some((mo) => mo === modeStr.toLowerCase())) {
        return modeStr as unknown as ThemeMode;
    }

    return 'auto';
}

export class ThemeToggle extends HTMLElement {
    // oxlint-disable-next-line typescript/no-non-null-assertion
    private html = document.querySelector('html')!;
    public static modes = ['auto', 'light', 'dark'];

    public constructor() {
        super();
        const ls = parseThemeMode(localStorage.getItem('theme-mode'));
        this.setMode(ls);
        const shadow = this.attachShadow({ mode: 'open' });

        const select = document.createElement('select');
        for (const mode of modes) {
            const el = document.createElement('option');
            el.value = mode;
            el.textContent = mode;
            if (mode === ls) {
                el.selected = true;
            }
            select.append(el);
        }

        select.addEventListener('change', (ev: Event) => {
            if (ev.target instanceof HTMLSelectElement) {
                this.setMode(parseThemeMode(ev.target.value));
            }
        });
        shadow.append(select);
    }

    private setMode(mode: ThemeMode) {
        localStorage.setItem('theme-mode', mode);
        switch (mode) {
            case 'dark':
                this.html.style.setProperty('color-scheme', 'dark');
                document.documentElement.style.setProperty(
                    '--system-cs-is-light',
                    '0',
                );
                break;
            case 'light':
                this.html.style.setProperty('color-scheme', 'light');
                document.documentElement.style.setProperty(
                    '--system-cs-is-light',
                    '1',
                );
                break;
            case 'auto': {
                const isLightMode = globalThis.matchMedia(
                    '(prefers-color-scheme: light)',
                ).matches;
                this.html.style.setProperty('color-scheme', 'light dark');
                document.documentElement.style.setProperty(
                    '--system-cs-is-light',
                    isLightMode ? '1' : '0',
                );
                break;
            }
            // No default
        }
    }
}
