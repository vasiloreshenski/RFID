import { Injectable } from '@angular/core';

@Injectable()
export class HtmlService {
    constructor() { }
    public static selectOption(select: HTMLSelectElement, expectedOptionValue: String) {
        for (let i = 0; i < select.options.length; i++) {
            const opt = select.options[i];
            if (opt.value === expectedOptionValue) {
                opt.selected = true;
            }
        }
    }
}
