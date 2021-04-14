using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Commands
{
    public interface IAttendanceLetterCommands
    {
        Task<AttendanceLetters> SaveLetter(AttendanceLetters letter);
        Task<AttendanceLetters> UpdateLetter(AttendanceLetters letter);
        Task<List<AttendanceLetters>> UpdateLetterBulk(List<AttendanceLetters> letters);
    }

    public class AttendanceLetterCommands : IAttendanceLetterCommands

    {
        private readonly Student360Context _db;
        private readonly IAuthenticationProvider _auth;

        public AttendanceLetterCommands(Student360Context db, IAuthenticationProvider auth)
        {
            _db = db;
            _auth = auth;
        }

        public async Task<AttendanceLetters> SaveLetter(AttendanceLetters letter)
        {
            _db.AttendanceLetters.Add(letter);
            await _db.SaveChangesAsync();

            return letter;
        }

        public async Task<AttendanceLetters> UpdateLetter(AttendanceLetters letter)
        {
            _db.AttendanceLetters.Update(letter);
            await _db.SaveChangesAsync();

            return letter;
        }


        public async Task<List<AttendanceLetters>> UpdateLetterBulk(List<AttendanceLetters> letters)
        {
            _db.AttendanceLetters.UpdateRange(letters);
            await _db.SaveChangesAsync();

            return letters;
        }
    }
}
