export class MemberCacheMap extends Map {
    public flattenResults() {
        return [...this.values()]
            .reduce((arr, elem) => elem?.result ? [...arr, ...elem.result] : arr, []);
    }
}