using E_Government.Application.DTO.User;
using E_Government.Application.ServiceContracts;
using E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure;
using E_Government.Domain.Entities.Enums;

namespace E_Government.Application.Services.NIDValidation
{
    public class NIDValidationService : INIDValidationService
    {
        private readonly IGovernorateService _governorateService;

        public NIDValidationService(IGovernorateService governorateService)
        {
            _governorateService = governorateService;
        }

        public bool ValidateNID(string NID)
        {
            if (NID.Length == 14)
            {
                return true;
            }

            return false;
        }

        public GovernorateDto ExtractGovernorateInfo(string NID)
        {
            int governorateCode = int.Parse(NID.Substring(0, 2)); // أول رقمين
            bool isValid = _governorateService.IsValidGovernorateCode(governorateCode);
            if (!isValid)
            {
                return new GovernorateDto
                {
                    Name = "غير محدد"
                };
            }

            var governorateName = _governorateService.GetGovernorateName(governorateCode);

            return new GovernorateDto
            {
                Name = governorateName,
            };
        }

        public Gender ExtractGender(string NID)
        {
            int genderCode = int.Parse(NID.Substring(12, 1)); // الرقم 13 وليس الرقم 8
            if (genderCode % 2 == 0)
                return Gender.Female;

            return Gender.Male;
        }

        public DateOnly ExtractDateOfBirth(string NID)
        {
            string birthDateNID = NID.Substring(1, 6); // YYMMDD
            int year = int.Parse(birthDateNID.Substring(0, 2));
            int month = int.Parse(birthDateNID.Substring(2, 2));
            int day = int.Parse(birthDateNID.Substring(4, 2));

            int fullYear;
            if (year < 50)
            {
                fullYear = 2000 + year;
            }
            else
            {
                fullYear = 1900 + year;
            }

            return new DateOnly(fullYear, month, day);
        }

        //// الطريقة الأصلية
        //public int CalculateCheckDigitOriginal(string NID)
        //{
        //    string first13Digits = NID.Substring(0, 13);
        //    int sum = 0;

        //    for (int i = 0; i < 13; i++)
        //    {
        //        int digit = int.Parse(first13Digits[i].ToString());
        //        int multiplier = (i % 2 == 0) ? 2 : 1;
        //        int product = digit * multiplier;

        //        if (product > 9)
        //        {
        //            product = (product / 10) + (product % 10);
        //        }

        //        sum += product;
        //    }

        //    int remainder = sum % 10;
        //    return remainder == 0 ? 0 : 10 - remainder;
        //}

        //// طريقة Luhn المعدلة
        //public int CalculateCheckDigitLuhn(string NID)
        //{
        //    string first13Digits = NID.Substring(0, 13);
        //    int sum = 0;

        //    for (int i = 0; i < 13; i++)
        //    {
        //        int digit = int.Parse(first13Digits[i].ToString());

        //        // في Luhn نبدأ من اليمين، لكن هنا نعكس المنطق
        //        if (i % 2 == 1) // الأرقام في المواضع الزوجية (من اليسار)
        //        {
        //            digit *= 2;
        //            if (digit > 9)
        //                digit = digit / 10 + digit % 10;
        //        }

        //        sum += digit;
        //    }

        //    return (10 - (sum % 10)) % 10;
        //}

        //// طريقة بديلة 3
        //public int CalculateCheckDigitAlternative(string NID)
        //{
        //    string first13Digits = NID.Substring(0, 13);
        //    int sum = 0;
        //    int[] weights = { 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 };

        //    for (int i = 0; i < 13; i++)
        //    {
        //        int digit = int.Parse(first13Digits[i].ToString());
        //        int product = digit * weights[i];

        //        if (product >= 10)
        //        {
        //            product = product / 10 + product % 10;
        //        }

        //        sum += product;
        //    }

        //    return (10 - (sum % 10)) % 10;
        //}

        //public int CalculateCheckDigit(string NID)
        //{
        //    // جرب الطرق المختلفة
        //    int actualDigit = int.Parse(NID.Substring(13, 1));

        //    int method1 = CalculateCheckDigitOriginal(NID);
        //    int method2 = CalculateCheckDigitLuhn(NID);
        //    int method3 = CalculateCheckDigitAlternative(NID);

        //    // أرجع الطريقة التي تطابق الرقم الفعلي
        //    if (method1 == actualDigit) return method1;
        //    if (method2 == actualDigit) return method2;
        //    if (method3 == actualDigit) return method3;

        //    // إذا لم تنجح أي طريقة، أرجع الطريقة الأصلية
        //    return method1;
        //}

        public NIDValidationResultDto ValidateAndExtractNID(string NID)
        {
            var result = new NIDValidationResultDto();

            // التحقق من الطول
            if (NID.Length != 14)
            {
                result.IsValid = false;
                result.Errors.Add("الرقم القومي يجب أن يكون 14 رقم");
                return result;
            }

            // التحقق من الأرقام
            if (!NID.All(char.IsDigit))
            {
                result.IsValid = false;
                result.Errors.Add("الرقم القومي يجب أن يحتوي على أرقام فقط");
                return result;
            }

            // التحقق من التاريخ
            if (!IsValidDateOfBirth(NID))
            {
                result.IsValid = false;
                result.Errors.Add("تاريخ الميلاد غير صحيح");
                return result;
            }

            // التحقق من المحافظة
            if (!IsValidGovernorateCode(NID))
            {
                result.IsValid = false;
                result.Errors.Add("كود المحافظة غير صحيح");
                return result;
            }

            //// التحقق من Check Digit بطرق متعددة
            //if (!IsValidCheckDigitMultiple(NID))
            //{
            //    result.IsValid = false;
            //    result.Errors.Add("الرقم القومي غير صحيح (Check Digit)");
            //    return result;
            //}

            // لو كله صح، استخرج البيانات
            result.IsValid = true;
            result.DateOfBirth = ExtractDateOfBirth(NID);
            result.Gender = ExtractGender(NID);
            result.Governorate = ExtractGovernorateInfo(NID);

            return result;
        }

        //private bool IsValidCheckDigitMultiple(string NID)
        //{
        //    int actualDigit = int.Parse(NID.Substring(13, 1));

        //    // جرب الطرق المختلفة
        //    int method1 = CalculateCheckDigitOriginal(NID);
        //    int method2 = CalculateCheckDigitLuhn(NID);
        //    int method3 = CalculateCheckDigitAlternative(NID);

        //    return actualDigit == method1 || actualDigit == method2 || actualDigit == method3;
        //}

        //private bool IsValidCheckDigit(string NID)
        //{
        //    int calculatedDigit = CalculateCheckDigit(NID);
        //    int actualDigit = int.Parse(NID.Substring(13, 1));

        //    return calculatedDigit == actualDigit;
        //}

        private bool IsValidGovernorateCode(string NID)
        {
            int governorateCode = int.Parse(NID.Substring(0, 2)); // أول رقمين
            return _governorateService.IsValidGovernorateCode(governorateCode);
        }

        private bool IsValidDateOfBirth(string NID)
        {
            try
            {
                string dateStr = NID.Substring(1, 6); // YYMMDD

                int year = int.Parse(dateStr.Substring(0, 2));
                int month = int.Parse(dateStr.Substring(2, 2));
                int day = int.Parse(dateStr.Substring(4, 2));

                int fullYear = year < 50 ? 2000 + year : 1900 + year;

                if (month < 1 || month > 12) return false;
                if (day < 1 || day > 31) return false;

                if (!IsValidDayForMonth(day, month, fullYear)) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidDayForMonth(int day, int month, int year)
        {
            int[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            if (month == 2 && IsLeapYear(year))
                return day <= 29;

            return day <= daysInMonth[month - 1];
        }

        private bool IsLeapYear(int year)
        {
            return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
        }

        public Status ExtractStatus(GovernorateDto governorate)
        {
            if (governorate.Name== "غير محدد")
            {
                return Status.Resident;
            }
            return Status.Civil;
        }
    }
}