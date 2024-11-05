import { Injectable } from '@angular/core';

@Injectable()
export class ProductService {
    getProductsData() {
        return [
            {
                name: 'Unlimited Inbox',
                description: 'With our site, you can manage your income and expenses, plan your budget, control loans, and receive real-time expense reports.',
                image: 'i-icons-login-live-collaboration',
            },
            {
                name: 'Data Security',
                description: 'Your data is securely protected and anonymized, ensuring maximum security.',
                image: 'i-icons-login-security',
            },
            {
                name: 'Cloud Backup',
                description: 'Use our service both on the website and through a convenient mobile app.',
                image: 'i-icons-login-subscribe',
            },
        ];
    }

    getProducts() {
        return Promise.resolve(this.getProductsData());
    }
};
