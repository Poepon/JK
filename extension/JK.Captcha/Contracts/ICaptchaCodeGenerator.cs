﻿namespace JK.Captcha.Contracts
{
    public interface ICaptchaCodeGenerator
    {
        string OutputText(bool hasLowerLetter, bool hasUpperLetter, bool hasNumber, int len);
    }
}