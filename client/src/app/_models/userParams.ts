import { User } from "./user";

export class UserParams {
    minAge = 18;
    maxAge = 99;
    currentPage = 1;
    itemsPerPage = 4;
    gender: string;
    orderBy = 'lastActive'

    constructor(user: User){
        this.gender = user.gender === 'female' ? 'male' : 'female';
    }
}