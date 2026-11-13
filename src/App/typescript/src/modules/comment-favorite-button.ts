export class CommentFavoriteButton extends HTMLElement {
    public static observedAttributes = ['post', 'comment', 'initial'] as const;

    private pre = document.createTextNode('[');
    private detail = document.createElement('a');
    private btn = document.createElement('button');
    private post = document.createTextNode(']');
    private msg = document.createElement('span');

    public favorited = false;

    public constructor() {
        // Always call super first in constructor
        super();
        const shadow = this.attachShadow({ mode: 'open' });

        const postNum = this.getAttribute('post');
        const commentID = this.getAttribute('comment');
        this.detail.href = `/${postNum}/${commentID}/favorites`;
        this.btn.className = 'btn-link fav-btn';
        this.msg.className = 'fav-details';

        this.btn.addEventListener('click', () => this.toggleFavorite());
        this.setBtnText(this.getAttribute('initial'));

        this.buildDOM(shadow);
    }

    private buildDOM(shadow: ShadowRoot) {
        const sheet = new CSSStyleSheet();
        sheet.replaceSync(`
.btn-link { background-color: transparent; border: 0; text-decoration: underline; }
.fav-btn { font-size: 0.8rem }
.fav-details { padding-left: 0.5rem }
.invis { display: none; }`);

        shadow.adoptedStyleSheets.push(sheet);
        shadow.append(this.pre);
        shadow.append(this.detail);
        shadow.append(this.btn);
        shadow.append(this.post);
        shadow.append(this.msg);
    }

    private async toggleFavorite() {
        const postNum = this.getAttribute('post');
        const commentID = this.getAttribute('comment');
        if (!commentID || !postNum) {
            return;
        }

        const method = this.favorited ? 'DELETE' : 'POST';
        const result = await fetch(`/${postNum}/${commentID}/favorite`, {
            method,
        });
        if (!result.ok) {
            return;
        }

        this.favorited = !this.favorited;
        const data = await result.json();
        this.setBtnText(data.currentCount);
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

    private setBtnText(count: number | string | null): undefined {
        let countNum = 0;

        if (typeof count === 'number') {
            countNum = count;
        } else {
            countNum = Math.trunc(Number(count ?? ''));
            if (Number.isNaN(countNum)) {
                countNum = 0;
            }
        }

        this.detail.textContent = countNum > 0 ? `${countNum} favorites` : '';
        this.btn.title = this.favorited
            ? 'remove favorite from this post'
            : 'add favorite to this post';

        this.btn.textContent = this.favorited ? '-' : '+';
    }
}
