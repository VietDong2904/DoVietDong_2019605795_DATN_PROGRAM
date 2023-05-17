import { RouterModule, Routes } from "@angular/router";
import { NgModule } from "@angular/core";
import { LayoutPageComponent } from "./layout-page/layout-page.component";
import { AboutUsComponent } from "./about-us/about-us.component";
import { ContactComponent } from "./contact/contact.component";
import { HomePageComponent } from "./home-page/home-page.component";
import { ShopPageComponent } from "./shop-page/shop-page.component";
import { CartDetailComponent } from "./cart-detail/cart-detail.component";
import { CheckoutComponent } from "./checkout/checkout.component";
import { ProductDetailComponent } from "./product-detail/product-detail.component";
import { AccountDetailComponent } from "./account-detail/account-detail.component";
import { ConfirmComponent } from "./confirm/confirm.component";

const routes: Routes = [
    {
        path: '',
        component: LayoutPageComponent,
        children: [
            {
                path: '',
                component: HomePageComponent,
            },
            {
                path: 'account-detail',
                component: AccountDetailComponent,
            },
            {
                path: 'about-us',
                component: AboutUsComponent
            },
            {
                path: 'contact',
                component: ContactComponent
            },
            {
                path: 'shop',
                component: ShopPageComponent
            },
            {
                path: 'cart',
                component: CartDetailComponent
            },
            {
                path: 'checkout',
                component: CheckoutComponent
            },
            {
                path: 'product-detail/:id',
                component: ProductDetailComponent,
            },
            {
                path: 'confirm/:id',
                component: ConfirmComponent,
                data: { title: 'Đặt hàng thành công' },
              },
        ]
    }
]

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
  })
  export class ElectronicRoutingModule { }