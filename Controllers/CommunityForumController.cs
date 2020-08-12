using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SterlingCommunityAPI.Models;

namespace SterlingCommunityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityForumController : ControllerBase
    {
        private readonly SterlingCommunityDBContext _context;
        private bool hasUserInsertedSessionkey;
        private bool doesKeymatch;
        private (bool isEntered, Session sess, bool isMatch) responseCheck;

        public CommunityForumController(SterlingCommunityDBContext context)
        {
            _context = context;
        }

        // GET: api/CommunityForum
        [HttpGet]
        [Route("GetAllSession")]
        public async Task<ActionResult<IEnumerable<Session>>> GetSession()
        {
            return await _context.Session.ToListAsync();
        }

        // GET: api/CommunityForum/5
        [HttpGet]
        [Route("GetAllSessionByID")]

        public async Task<ActionResult<Session>> GetSession(int id)
        {
            var session = await _context.Session.FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            return session;
        }

        // PUT: api/CommunityForum/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        [Route("PutSessionByID")]

        public async Task<IActionResult> PutSession(int id, Session session)
        {
            if (id != session.SessionId)
            {
                return BadRequest();
            }

            _context.Entry(session).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SessionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CommunityForum
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Route("SaveSession")]
        public async Task<ActionResult<Session>> PostSession(Session session)
        {
            try
            {
                _context.Session.Add(session);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {


            }

            return CreatedAtAction("GetSession", new { id = session.SessionId }, session);
        }

        [HttpPost]
        [Route("KeyInSessionByUser")]
        public async Task<ActionResult> KeyInSessionByUser(Session session)
        {
            try
            {
                var sess = (_context.Session.Single(x => x.SessionKey.ToLower() == session.SessionKeyInsertedByUser.ToLower()));
                if (sess == null)
                {
                    return Ok(false);
                }
                sess.SessionKeyInsertedByUser = session.SessionKeyInsertedByUser;
                sess.DateSessionKeyInsertedByUser = DateTime.Now;
                sess.DateIsActiveChanged = DateTime.Now;
                sess.IsActive = true;
                sess.AccountNumber = session.AccountNumber;
                sess.Email = session.Email;
                sess.Bvn = session.Bvn;
                sess.MobileNumber = session.MobileNumber;
                sess.UserName = session.UserName;
                _context.Session.Update(sess);
                await _context.SaveChangesAsync();
                return Ok(true);

            }
            catch (Exception ex)
            {

                return Ok(false);

            }

        }



        [HttpPost]
        [Route("PollSession")]
        public async Task<ActionResult> PollSession(Session session)
        {
            try
            {

                var start = DateTime.Now.TimeOfDay;
                var cutOff = DateTime.Now.AddMinutes(3);
                var cutOffTime = start.Add(new System.TimeSpan(0, 3, 0));


                while (DateTime.Now.TimeOfDay < cutOffTime)
                {
                    if (hasUserInsertedSessionkey)
                    {
                        break;
                    }
                    responseCheck = CheckifUserHasResponded(session);
                }
                if (responseCheck.isEntered)
                {

                    //return user full info
                    return Ok(new { isSuccess = true, SessionLogin = responseCheck.sess });


                }
                else
                {
                    //return user should try again as time has expired
                    return Ok(new { isSuccess = false, SessionLogin = responseCheck.sess });

                }
            }
            catch (Exception ex)
            {
                return Ok(new { isSuccess = true, SessionLogin = new Session() });


            }


        }

        private (bool isEntered, Session sess, bool isMatch) CheckifUserHasResponded(Session sess)
        {

            var session = _context.Session.Where(d => d.SessionKeyInsertedByUser.ToLower().Equals(sess.SessionKey.ToLower())).FirstOrDefault();
            if (session == null)
            {
                hasUserInsertedSessionkey = false;
                return (hasUserInsertedSessionkey, sess, doesKeymatch);

            }
            else
            {
                hasUserInsertedSessionkey = true;
                return (true, session, true);
            }
            //if (string.IsNullOrEmpty(session.SessionKeyInsertedByUser))
            //{
            //    hasUserInsertedSessionkey = false;
            //    return (hasUserInsertedSessionkey, sess, doesKeymatch);


            //}
            //else
            //{
            //    if (session.SessionKey==session.SessionKeyInsertedByUser)
            //    {
            //        hasUserInsertedSessionkey = true;
            //        sess = session;
            //        doesKeymatch = true;
            //    }
            //}

            return (hasUserInsertedSessionkey, sess, doesKeymatch);
        }

        // DELETE: api/CommunityForum/5
        [HttpDelete]
        [Route("DeleteSessionByID")]

        public async Task<ActionResult<Session>> DeleteSession(int id)
        {
            var session = await _context.Session.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            _context.Session.Remove(session);
            await _context.SaveChangesAsync();

            return session;
        }

        private bool SessionExists(int id)
        {
            return _context.Session.Any(e => e.SessionId == id);
        }
    }
}
