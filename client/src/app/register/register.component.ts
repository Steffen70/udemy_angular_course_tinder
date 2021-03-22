import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() accountService: AccountService;
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;

  model: any = {}

  constructor() { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])
    });
    this.registerForm.controls.password.valueChanges.subscribe(() =>
      this.registerForm.controls.confirmPassword.updateValueAndValidity())
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
        ? null
        : { isMatching: true };
    }
  }

  register() {
    this.accountService.register(this.registerForm.value).subscribe(() => this.cancel());
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
