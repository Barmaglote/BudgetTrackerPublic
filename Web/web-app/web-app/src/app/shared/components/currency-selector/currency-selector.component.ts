import { Component, EventEmitter, Input, OnDestroy, Output, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { translate } from '@ngneat/transloco';
import { ConfirmationService } from 'primeng/api';
import { Subject, Subscription } from 'rxjs';

@Component({
  selector: 'app-currency-selector',
  templateUrl: './currency-selector.component.html',
  styleUrls: ['./currency-selector.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => CurrencySelectorComponent),
    multi: true,
  }],
})
export class CurrencySelectorComponent implements OnDestroy {
  @Input() currency: string | undefined;
  @Output() currencyChange = new EventEmitter<string>();
  private _valueChangesSubscription: Subscription | undefined;
  private _touchesSubscription: Subscription | undefined;

  constructor(private confirmationService: ConfirmationService) {}
  ngOnDestroy(): void {
    if (this._valueChangesSubscription) {
      this._valueChangesSubscription.unsubscribe();
    }

    if (this._touchesSubscription) {
      this._touchesSubscription.unsubscribe();
    }
  }

  public disabled = false;
  private touches = new Subject();
  private valueChanges = new Subject<string>();

  writeValue(currency: string): void {
    this.selectedCurrency = currency;
  }
  registerOnChange(fn: any): void {
    this._valueChangesSubscription = this.valueChanges.subscribe(fn);
  }
  registerOnTouched(fn: any): void {
    this._touchesSubscription = this.touches.subscribe(fn);
  }
  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  public selectedCurrency: string | undefined = '';
  public currencyOptions = [
    { label: 'AED - United Arab Emirates Dirham', value: 'AED' },
    { label: 'AFN - Afghan Afghani', value: 'AFN' },
    { label: 'ALL - Albanian Lek', value: 'ALL' },
    { label: 'AMD - Armenian Dram', value: 'AMD' },
    { label: 'ANG - Netherlands Antillean Guilder', value: 'ANG' },
    { label: 'AOA - Angolan Kwanza', value: 'AOA' },
    { label: 'ARS - Argentine Peso', value: 'ARS' },
    { label: 'AUD - Australian Dollar', value: 'AUD' },
    { label: 'AWG - Aruban Florin', value: 'AWG' },
    { label: 'AZN - Azerbaijani Manat', value: 'AZN' },
    { label: 'BAM - Bosnia-Herzegovina Convertible Mark', value: 'BAM' },
    { label: 'BBD - Barbadian Dollar', value: 'BBD' },
    { label: 'BDT - Bangladeshi Taka', value: 'BDT' },
    { label: 'BGN - Bulgarian Lev', value: 'BGN' },
    { label: 'BHD - Bahraini Dinar', value: 'BHD' },
    { label: 'BIF - Burundian Franc', value: 'BIF' },
    { label: 'BMD - Bermudian Dollar', value: 'BMD' },
    { label: 'BND - Brunei Dollar', value: 'BND' },
    { label: 'BOB - Bolivian Boliviano', value: 'BOB' },
    { label: 'BRL - Brazilian Real', value: 'BRL' },
    { label: 'BSD - Bahamian Dollar', value: 'BSD' },
    { label: 'BTN - Bhutanese Ngultrum', value: 'BTN' },
    { label: 'BWP - Botswanan Pula', value: 'BWP' },
    { label: 'BYN - Belarusian Ruble', value: 'BYN' },
    { label: 'BZD - Belize Dollar', value: 'BZD' },
    { label: 'CAD - Canadian Dollar', value: 'CAD' },
    { label: 'CDF - Congolese Franc', value: 'CDF' },
    { label: 'CHF - Swiss Franc', value: 'CHF' },
    { label: 'CLP - Chilean Peso', value: 'CLP' },
    { label: 'CNY - Chinese Yuan', value: 'CNY' },
    { label: 'COP - Colombian Peso', value: 'COP' },
    { label: 'CRC - Costa Rican Colón', value: 'CRC' },
    { label: 'CUP - Cuban Peso', value: 'CUP' },
    { label: 'CVE - Cape Verdean Escudo', value: 'CVE' },
    { label: 'CZK - Czech Republic Koruna', value: 'CZK' },
    { label: 'DJF - Djiboutian Franc', value: 'DJF' },
    { label: 'DKK - Danish Krone', value: 'DKK' },
    { label: 'DOP - Dominican Peso', value: 'DOP' },
    { label: 'DZD - Algerian Dinar', value: 'DZD' },
    { label: 'EGP - Egyptian Pound', value: 'EGP' },
    { label: 'ERN - Eritrean Nakfa', value: 'ERN' },
    { label: 'ETB - Ethiopian Birr', value: 'ETB' },
    { label: 'EUR - Euro', value: 'EUR' },
    { label: 'FJD - Fijian Dollar', value: 'FJD' },
    { label: 'FKP - Falkland Islands Pound', value: 'FKP' },
    { label: 'FOK - Faroese Króna', value: 'FOK' },
    { label: 'GBP - British Pound Sterling', value: 'GBP' },
    { label: 'GEL - Georgian Lari', value: 'GEL' },
    { label: 'GGP - Guernsey Pound', value: 'GGP' },
    { label: 'GHS - Ghanaian Cedi', value: 'GHS' },
    { label: 'GIP - Gibraltar Pound', value: 'GIP' },
    { label: 'GMD - Gambian Dalasi', value: 'GMD' },
    { label: 'GNF - Guinean Franc', value: 'GNF' },
    { label: 'GTQ - Guatemalan Quetzal', value: 'GTQ' },
    { label: 'GYD - Guyanaese Dollar', value: 'GYD' },
    { label: 'HKD - Hong Kong Dollar', value: 'HKD' },
    { label: 'HNL - Honduran Lempira', value: 'HNL' },
    { label: 'HRK - Croatian Kuna', value: 'HRK' },
    { label: 'HTG - Haitian Gourde', value: 'HTG' },
    { label: 'HUF - Hungarian Forint', value: 'HUF' },
    { label: 'IDR - Indonesian Rupiah', value: 'IDR' },
    { label: 'ILS - Israeli New Shekel', value: 'ILS' },
    { label: 'IMP - Isle of Man Pound', value: 'IMP' },
    { label: 'INR - Indian Rupee', value: 'INR' },
    { label: 'IQD - Iraqi Dinar', value: 'IQD' },
    { label: 'IRR - Iranian Rial', value: 'IRR' },
    { label: 'ISK - Icelandic Króna', value: 'ISK' },
    { label: 'JEP - Jersey Pound', value: 'JEP' },
    { label: 'JMD - Jamaican Dollar', value: 'JMD' },
    { label: 'JOD - Jordanian Dinar', value: 'JOD' },
    { label: 'JPY - Japanese Yen', value: 'JPY' },
    { label: 'KES - Kenyan Shilling', value: 'KES' },
    { label: 'KGS - Kyrgystani Som', value: 'KGS' },
    { label: 'KHR - Cambodian Riel', value: 'KHR' },
    { label: 'KID - Kiribati Dollar', value: 'KID' },
    { label: 'KMF - Comorian Franc', value: 'KMF' },
    { label: 'KRW - South Korean Won', value: 'KRW' },
    { label: 'KWD - Kuwaiti Dinar', value: 'KWD' },
    { label: 'KYD - Cayman Islands Dollar', value: 'KYD' },
    { label: 'KZT - Kazakhstani Tenge', value: 'KZT' },
    { label: 'LAK - Laotian Kip', value: 'LAK' },
    { label: 'LBP - Lebanese Pound', value: 'LBP' },
    { label: 'LKR - Sri Lankan Rupee', value: 'LKR' },
    { label: 'LRD - Liberian Dollar', value: 'LRD' },
    { label: 'LSL - Lesotho Loti', value: 'LSL' },
    { label: 'LYD - Libyan Dinar', value: 'LYD' },
    { label: 'MAD - Moroccan Dirham', value: 'MAD' },
    { label: 'MDL - Moldovan Leu', value: 'MDL' },
    { label: 'MGA - Malagasy Ariary', value: 'MGA' },
    { label: 'MKD - Macedonian Denar', value: 'MKD' },
    { label: 'MMK - Myanma Kyat', value: 'MMK' },
    { label: 'MNT - Mongolian Tugrik', value: 'MNT' },
    { label: 'MOP - Macanese Pataca', value: 'MOP' },
    { label: 'MRU - Mauritanian Ouguiya', value: 'MRU' },
    { label: 'MUR - Mauritian Rupee', value: 'MUR' },
    { label: 'MVR - Maldivian Rufiyaa', value: 'MVR' },
    { label: 'MWK - Malawian Kwacha', value: 'MWK' },
    { label: 'MXN - Mexican Peso', value: 'MXN' },
    { label: 'MYR - Malaysian Ringgit', value: 'MYR' },
    { label: 'MZN - Mozambican Metical', value: 'MZN' },
    { label: 'NAD - Namibian Dollar', value: 'NAD' },
    { label: 'NGN - Nigerian Naira', value: 'NGN' },
    { label: 'NIO - Nicaraguan Córdoba', value: 'NIO' },
    { label: 'NOK - Norwegian Krone', value: 'NOK' },
    { label: 'NPR - Nepalese Rupee', value: 'NPR' },
    { label: 'NZD - New Zealand Dollar', value: 'NZD' },
    { label: 'OMR - Omani Rial', value: 'OMR' },
    { label: 'PAB - Panamanian Balboa', value: 'PAB' },
    { label: 'PEN - Peruvian Nuevo Sol', value: 'PEN' },
    { label: 'PGK - Papua New Guinean Kina', value: 'PGK' },
    { label: 'PHP - Philippine Peso', value: 'PHP' },
    { label: 'PKR - Pakistani Rupee', value: 'PKR' },
    { label: 'PLN - Polish Złoty', value: 'PLN' },
    { label: 'PYG - Paraguayan Guarani', value: 'PYG' },
    { label: 'QAR - Qatari Rial', value: 'QAR' },
    { label: 'RON - Romanian Leu', value: 'RON' },
    { label: 'RSD - Serbian Dinar', value: 'RSD' },
    { label: 'RUB - Russian Ruble', value: 'RUB' },
    { label: 'RWF - Rwandan Franc', value: 'RWF' },
    { label: 'SAR - Saudi Riyal', value: 'SAR' },
    { label: 'SBD - Solomon Islands Dollar', value: 'SBD' },
    { label: 'SCR - Seychellois Rupee', value: 'SCR' },
    { label: 'SDG - Sudanese Pound', value: 'SDG' },
    { label: 'SEK - Swedish Krona', value: 'SEK' },
    { label: 'SGD - Singapore Dollar', value: 'SGD' },
    { label: 'SHP - Saint Helena Pound', value: 'SHP' },
    { label: 'SLL - Sierra Leonean Leone', value: 'SLL' },
    { label: 'SOS - Somali Shilling', value: 'SOS' },
    { label: 'SRD - Surinamese Dollar', value: 'SRD' },
    { label: 'SSP - South Sudanese Pound', value: 'SSP' },
    { label: 'STN - São Tomé and Príncipe Dobra', value: 'STN' },
    { label: 'SVC - Salvadoran Colón', value: 'SVC' },
    { label: 'SZL - Eswatini Lilangeni', value: 'SZL' },
    { label: 'THB - Thai Baht', value: 'THB' },
    { label: 'TJS - Tajikistani Somoni', value: 'TJS' },
    { label: 'TMT - Turkmenistani Manat', value: 'TMT' },
    { label: 'TND - Tunisian Dinar', value: 'TND' },
    { label: 'TOP - Tongan Paʻanga', value: 'TOP' },
    { label: 'TRY - Turkish Lira', value: 'TRY' },
    { label: 'TTD - Trinidad and Tobago Dollar', value: 'TTD' },
    { label: 'TWD - New Taiwan Dollar', value: 'TWD' },
    { label: 'TZS - Tanzanian Shilling', value: 'TZS' },
    { label: 'UAH - Ukrainian Hryvnia', value: 'UAH' },
    { label: 'UGX - Ugandan Shilling', value: 'UGX' },
    { label: 'USD - United States Dollar', value: 'USD' },
    { label: 'UYU - Uruguayan Peso', value: 'UYU' },
    { label: 'UZS - Uzbekistan Som', value: 'UZS' },
    { label: 'VES - Venezuelan Bolívar', value: 'VES' },
    { label: 'VND - Vietnamese Đồng', value: 'VND' },
    { label: 'VUV - Vanuatu Vatu', value: 'VUV' },
    { label: 'WST - Samoan Tala', value: 'WST' },
    { label: 'XAF - Central African CFA Franc', value: 'XAF' },
    { label: 'XCD - East Caribbean Dollar', value: 'XCD' },
    { label: 'XDR - Special Drawing Rights', value: 'XDR' },
    { label: 'XOF - West African CFA franc', value: 'XOF' },
    { label: 'XPF - CFP Franc', value: 'XPF' },
    { label: 'YER - Yemeni Rial', value: 'YER' },
    { label: 'ZAR - South African Rand', value: 'ZAR' },
    { label: 'ZMW - Zambian Kwacha', value: 'ZMW' },
    { label: 'ZWL - Zimbabwean Dollar', value: 'ZWL' }
  ];

  onCurrencyChange() {
    this.currencyChange.emit(this.selectedCurrency);
  }
}
