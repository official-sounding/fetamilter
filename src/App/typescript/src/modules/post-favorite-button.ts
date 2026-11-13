export class PostFavoriteButton extends HTMLElement {
    public static observedAttributes = ['post', 'full-text'] as const;

    private pre = document.createElement('span');
    private btn = document.createElement('button');
    private post = document.createElement('span');
    private msg = document.createElement('span');

    private favorited = false;

    public constructor() {
        // Always call super first in constructor
        super();

        this.pre.textContent = '[';
        this.pre.textContent = ']';

        const shadow = this.attachShadow({ mode: 'open' });

        this.btn.className = 'btn-link fav-btn';
        this.msg.className = 'fav-details';

        this.btn.addEventListener('click', () => this.toggleFavorite());
        this.setBtnText();
        this.setWrapperClass();

        const sheet = new CSSStyleSheet();
        sheet.replaceSync(`
.btn-link { background-color: transparent; border: 0; text-decoration: underline; color: var(--link-color); }
.fav-btn { font-size: 0.8rem }
.fav-details { padding-left: 0.5rem }
.invis { display: none; }`);

        shadow.adoptedStyleSheets.push(sheet);
        shadow.append(this.pre);
        shadow.append(this.btn);
        shadow.append(this.post);
        shadow.append(this.msg);
    }

    private async toggleFavorite() {
        const postNum = this.getAttribute('post');
        if (!postNum) {
            return;
        }

        const method = this.favorited ? 'DELETE' : 'POST';
        const result = await fetch(`/${postNum}/favorite`, { method });
        if (!result.ok) {
            return;
        }

        this.favorited = !this.favorited;
        this.setBtnText();
        const data = await result.json();
        if (data.actionSuccessful) {
            this.msg.textContent = this.favorited
                ? 'favorite added'
                : 'favorite removed';
        } else {
            this.msg.textContent = this.favorited
                ? 'favorite already added'
                : 'favorite already removed';
        }
    }

    private setBtnText() {
        this.btn.title = this.favorited
            ? 'remove favorite from this post'
            : 'add favorite to this post';

        if (this.getAttribute('full-text') === 'true') {
            this.btn.textContent = this.favorited
                ? 'remove favorite'
                : 'add to favorites';
        } else {
            this.btn.textContent = this.favorited ? '-' : '+';
        }
    }

    private setWrapperClass() {
        if (this.getAttribute('full-text') === 'true') {
            this.pre.className = '';
            this.post.className = '';
        } else {
            this.pre.className = 'invis';
            this.post.className = 'invis';
        }
    }
}
