import { HttpParams } from "@angular/common/http";
import { User } from "./user";

export abstract class PaginationParams {
    minAge = 18;
    maxAge = 99;
    currentPage = 1;
    itemsPerPage = 12;

    public getIdentifier() {
        return Object.values(this).join('-');
    }

    public getPaginationHeaders() {
        let params = new HttpParams();

        for (let [key, value] of Object.entries(this))
            if (value != null)
                params = params.append(key, value.toString())

        return params
    }
}

export class UserParams extends PaginationParams {
    gender: string;
    orderBy = 'lastActive'

    constructor(user: User) {
        super();
        this.gender = user.gender === 'female' ? 'male' : 'female';
    }
}

export class LikesParams extends PaginationParams {
    predicate: string;

    constructor(predicate: string) {
        super();
        this.predicate = predicate;
    }
}

export class MessageParams extends PaginationParams {
    container: string;

    constructor(container: string) {
        super();
        this.container = container;
    }
}