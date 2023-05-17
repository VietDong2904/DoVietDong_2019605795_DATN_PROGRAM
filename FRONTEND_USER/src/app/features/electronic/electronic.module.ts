import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NZ_I18N, en_US, vi_VN } from 'ng-zorro-antd/i18n';
import { CommonModule } from '@angular/common';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { NzRateModule } from 'ng-zorro-antd/rate';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzCommentModule } from 'ng-zorro-antd/comment';
import { NzTreeModule } from 'ng-zorro-antd/tree';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzTreeSelectModule } from 'ng-zorro-antd/tree-select';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { HeaderPageComponent } from './header-page/header-page.component';
import { FooterPageComponent } from './footer-page/footer-page.component';
import { LayoutPageComponent } from './layout-page/layout-page.component';
import { LoginPageComponent } from './login-page/login-page.component';
import { BenefitsComponent } from './benefits/benefits.component';
import { AboutUsComponent } from './about-us/about-us.component';
import { ContactComponent } from './contact/contact.component';
import { HomePageComponent } from './home-page/home-page.component';
import { ShopPageComponent } from './shop-page/shop-page.component';
import { CartDetailComponent } from './cart-detail/cart-detail.component';
import { CheckoutComponent } from './checkout/checkout.component';
import { ProductDetailComponent } from './product-detail/product-detail.component';
import { AccountDetailComponent } from './account-detail/account-detail.component';
import { ConfirmComponent } from './confirm/confirm.component';
export function HttpLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(http, './assets/i18n/', '.json');
  }
@NgModule({
    declarations: [
        LayoutPageComponent,
        LoginPageComponent,
        HeaderPageComponent,
        FooterPageComponent,
        BenefitsComponent,
        AboutUsComponent,
        ContactComponent,
        HomePageComponent,
        ShopPageComponent,
        AccountDetailComponent,
        ProductDetailComponent,
        CartDetailComponent,
        CheckoutComponent,
        ConfirmComponent
    ],
    imports: [
        BrowserModule,
        FormsModule,
        ReactiveFormsModule,
        HttpClientModule,
        RouterModule,
        BrowserAnimationsModule,
        CommonModule,
        NzRadioModule,
        NzModalModule,
        NzFormModule,
        NzMessageModule,
        NzNotificationModule,
        NzAvatarModule,
        NzIconModule,
        NzButtonModule,
        NzUploadModule,
        NzRateModule,
        NzPaginationModule,
        NzTableModule,
        NzCardModule,
        NzCommentModule,
        NzTreeModule,
        NzSelectModule,
        NzTabsModule,
        NzEmptyModule,
        NzSpinModule,
        NzDividerModule,
        NzInputModule,
        NzTreeSelectModule,
        NzBadgeModule,
        NzLayoutModule
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    providers: [ { provide: NZ_I18N, useValue: vi_VN }],

})
export class ElectronicModule {}