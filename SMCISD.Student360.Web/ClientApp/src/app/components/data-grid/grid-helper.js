"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.calculateHeaders = exports.camelCaseToSentence = exports.camelCaseToPascalCase = exports.calculateOrderByForRequest = void 0;
function calculateOrderByForRequest(grid) {
    var result = [];
    var sortedHeaders = grid.headers.slice().sort(function (a, b) {
        if (a.orderNumber > b.orderNumber)
            return 1;
        if (b.orderNumber > a.orderNumber)
            return -1;
        return 0;
    });
    sortedHeaders.forEach(function (header) {
        if (header.order !== undefined) {
            result.push({ column: header.columnName, direction: header.order ? 'descending' : 'ascending' });
        }
    });
    return result;
}
exports.calculateOrderByForRequest = calculateOrderByForRequest;
function camelCaseToPascalCase(camelCaseWord) {
    return camelCaseWord.charAt(0).toUpperCase() + camelCaseWord.slice(1);
}
exports.camelCaseToPascalCase = camelCaseToPascalCase;
function camelCaseToSentence(camelCaseWord) {
    var result = camelCaseWord.replace(/([A-Z])/g, " $1");
    return camelCaseToPascalCase(result);
}
exports.camelCaseToSentence = camelCaseToSentence;
function calculateHeaders(grid) {
    var headers = [];
    grid.columns.forEach(function (col) {
        headers.push({ name: camelCaseToSentence(col), order: undefined, columnName: camelCaseToPascalCase(col), orderNumber: 0 });
    });
    return headers;
}
exports.calculateHeaders = calculateHeaders;
//# sourceMappingURL=grid-helper.js.map