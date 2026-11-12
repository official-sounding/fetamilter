export class CommentFavoriteButton extends HTMLElement {
    static observedAttributes = ["post", "comment", "initial"] as const;

    private pre = document.createElement("span", { });
    private detail = document.createElement("a");
    private btn = document.createElement("button");
    private post = document.createElement("span");
    private msg = document.createElement("span");

    favorited = false;

    constructor() {
        // Always call super first in constructor
        super();

        const shadow = this.attachShadow({ mode: "open" });

        this.pre.innerText = "[";
        this.post.innerText = "]";

        const postNum = this.getAttribute("post");
        const commentID = this.getAttribute("comment");

        this.detail.href = `/${postNum}/${commentID}/favorites`;
        this.btn.className = "btn-link fav-btn";
        this.msg.className = "fav-details";

        this.btn.addEventListener("click", () => this.toggleFavorite());
        this.setBtnText(this.getAttribute("initial"));

        const sheet = new CSSStyleSheet();
        sheet.replaceSync(`
.btn-link { background-color: transparent; border: 0; text-decoration: underline; }
.fav-btn { font-size: 0.8rem }
.fav-details { padding-left: 0.5rem }
.invis { display: none; }`);

        shadow.adoptedStyleSheets.push(sheet);
        shadow.appendChild(this.pre);
        shadow.appendChild(this.detail);
        shadow.appendChild(this.btn);
        shadow.appendChild(this.post);
        shadow.appendChild(this.msg);
    }

    async toggleFavorite() {
        const postNum = this.getAttribute("post");
        const commentID = this.getAttribute("comment");
        if (!commentID || !postNum) {
            return;
        }

        const method = this.favorited ? "DELETE" : "POST";
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
            this.msg.innerText = this.favorited
                ? "favorite added"
                : "favorite removed";
        } else {
            this.msg.innerText = this.favorited
                ? "favorite already added"
                : "favorite already removed";
        }
    }

    setBtnText(count: number | string | null) {
        if (typeof count !== "number") {
            count = parseInt(count ?? "");
            if (isNaN(count)) {
                count = 0;
            }
        }

        this.btn.title = this.favorited
            ? "remove favorite from this post"
            : "add favorite to this post";

        this.detail.innerText = count > 0 ? `${count} favorites ` : "";
        this.btn.innerText = this.favorited ? "-" : "+";
    }
}
