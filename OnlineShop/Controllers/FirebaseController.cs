using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using static OnlineShop.Controllers.FirebaseController;

namespace OnlineShop.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FirebaseController : ControllerBase
	{
		private readonly IFirebaseConfig config = new FirebaseConfig
		{
			AuthSecret = "5q94n5M3JkB6zq1ZqiKBPbkgGC4swED8cVLbgbBW",
			BasePath = "https://onlineshop-931f7-default-rtdb.asia-southeast1.firebasedatabase.app/"
		};
		private readonly IFirebaseClient _client;
		public FirebaseController()
		{
			_client = new FirebaseClient(config);
		}
		public class MessageModel
		{
			public string UserId { get; set; }
			public string SellerId { get; set; }
			public string Message { get; set; }
			public string SenderId { get; set; }
		}
            public class PeerModel
            {
                public string UserId { get; set; }
                public string PeerId { get; set; }
            }

            [HttpPost("SetMessage")]
        public async Task<IActionResult> SetMessageToFirebase([FromBody] MessageModel messageModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string nodeMessage = $"chats/chat_sellerId_{messageModel.SellerId}_userId_{messageModel.UserId}/messages";
                var chatData = new
                {
                    senderId = messageModel.SenderId,
                    content = messageModel.Message,
                    timeSend = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                PushResponse response = await _client.PushAsync(nodeMessage, chatData);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok("Chat created successfully");
                }
                else
                {
                    return BadRequest("Failed to create chat. Status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("GetMessages")]
		public async Task<ActionResult<IEnumerable<object>>> GetMessagesFromFirebase(string userId, string sellerId)
		{
			try
			{
				string nodeMessage = "chats/chat_sellerId_" + sellerId + "_userId_" + userId + "/messages";
				FirebaseResponse response = await _client.GetAsync(nodeMessage);

				if (response.Body != "null")
				{
					Dictionary<string, object> messageDict = response.ResultAs<Dictionary<string, object>>();
					List<object> messages = new List<object>();
					List<JObject> messages2 = new List<JObject>(); // Sử dụng List<JObject> thay vì List<object>

					foreach (var pair in messageDict)
					{
						// Chuyển đổi từ JSON sang đối tượng JObject
						JObject messageJson2 = JObject.Parse(pair.Value.ToString());
						messages2.Add(messageJson2);
					}
					// Sắp xếp danh sách tin nhắn theo trường 'timeSend'
					messages2 = messages2.OrderBy(m => DateTime.Parse((string)m["timeSend"])).ToList();
					foreach (var obj in messages2)
					{
						messages.Add(obj.ToString());

					}
					return messages;
				}
				else
				{
					return NotFound("No messages found");
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}
		[HttpGet("GetChatsBySellerId")]
		public async Task<ActionResult<IEnumerable<object>>> GetChatsBySellerId(string sellerId)
		{
			try
			{
				// Tạo đường dẫn tới node chats
				string nodeChats = "chats";

				// Truy vấn dữ liệu từ Firebase
				FirebaseResponse response = await _client.GetAsync(nodeChats);

				// Kiểm tra xem phản hồi có dữ liệu không
				if (response.Body != "null")
				{
					// Chuyển đổi dữ liệu phản hồi thành một đối tượng Dictionary
					Dictionary<string, object> chatsDict = response.ResultAs<Dictionary<string, object>>();
					List<object> chats = new List<object>();
					// Tạo danh sách để lưu trữ tin nhắn thỏa mãn điều kiện
					string lastMessage = "";
					// Lặp qua từng cặp key-value trong Dictionary
					foreach (var pair in chatsDict)
					{
						// Kiểm tra xem key của node chat có chứa sellerId không
						if (pair.Key.Contains("sellerId_" + sellerId))
						{
							// Lấy ra danh sách các tin nhắn từ node chat
							var chatMessages = pair.Value.ToString();
							
						
							// Phân tích chuỗi JSON
							JObject jsonObject = JObject.Parse(chatMessages);

							// Lấy danh sách tin nhắn từ thuộc tính "messages"
							var messages = jsonObject["messages"];

							// Nếu danh sách tin nhắn không rỗng, sắp xếp theo thời gian gửi và lấy tin nhắn cuối cùng
							if (messages != null)
							{
								var sortedMessages = messages.Values()
									.OrderByDescending(m => DateTime.Parse((string)m["timeSend"]))
									.ToList();

								if (sortedMessages.Any())
								{
									if (sortedMessages.First()["senderId"].ToString() == sellerId)
									{
										lastMessage = sortedMessages.ElementAt(1).ToString();
									}
									else
									{
										lastMessage = sortedMessages.ElementAt(0).ToString();

									}
									var mess = new
									{
										userId = ExtractUserId(pair.Key),
										message = lastMessage
									};
									chats.Add(mess);
								}
								else
								{
									return NotFound("No message found");
								}
							}
						}
					}
					return Ok(chats);
				}
				else
				{
					return NotFound("No chats found");
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}
        [HttpPost("SetPeerId")]
        public async Task<IActionResult> SetPeerIdToFirebase([FromBody] PeerModel peerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                string nodePeer = $"peer/userId_{peerModel.UserId}/peerId";
                SetResponse response = await _client.SetAsync(nodePeer, peerModel.PeerId);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok("peerId created successfully");
                }
                else
                {
                    return BadRequest("Failed to create chat. Status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("GetPeerId/{userId}")]
        public async Task<IActionResult> GetPeerIdFromFirebase(int userId)
        {
            try
            {
                string nodePeer = $"peer/userId_{userId}/peerId";
                var snapshot = await _client.GetAsync(nodePeer);

                if (snapshot.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var peerId = snapshot.Body.ToString();
                    string normalizedPeerId = peerId.Replace("\\", "").Replace("\"", "");
                    return Ok(normalizedPeerId);
                }
                else
                {
                    return NotFound("PeerId not found for userId: " + userId);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

		public class NoticeSend
		{
			public String receivedId { get; set; }
			public String content { get; set; }

        }
        public class UpdateSeen
        {
            public String userId { get; set; }
            public String noticeId { get; set; }

        }

        public class NoticeData
        {
            public String id { get; set; }
            public String message { get; set; }
            public Boolean seen { get; set; }

        }
        public class Body
        {
            public String name { get; set; }

        }

        [HttpPost("PushNotice")]
        public async Task<IActionResult> PushNoticeToFirebase([FromBody] NoticeSend notice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string nodeNotice = $"notices/customerId_{notice.receivedId}";
                var noticeData = new
                {
                    message = notice.content,
                    seen = false,
                };

                PushResponse response = await _client.PushAsync(nodeNotice, noticeData);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
					var body = JsonConvert.DeserializeObject<Body>(response.Body.ToString());
                    string nodeNoticeUpdate = $"notices/customerId_{notice.receivedId}/{body.name}";
					var noticeDataUpdate = new
					{
						id = body.name,
                        message = notice.content,
                        seen = false,
                    };
                    await _client.SetAsync(nodeNoticeUpdate, noticeDataUpdate);
                    return Ok("Notice created successfully");
                }
                else
                {
                    return BadRequest("Failed to create chat. Status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpGet("GetNotices/{userId}")]
        public async Task<IActionResult> GetNoticesFromFirebase(int userId)
        {
            try
            {
                // Create the path to the notices node
                string nodeNotice = $"notices/customerId_{userId}";

                // Query data from Firebase
                FirebaseResponse response = await _client.GetAsync(nodeNotice);

                // Check if the response contains data
                if (response.Body != "null")
                {
                    // Convert the response data to a dictionary
                    Dictionary<string, NoticeData> noticesDict = JsonConvert.DeserializeObject<Dictionary<string, NoticeData>>(response.Body);
                    List<NoticeData> noticeList = new List<NoticeData>();

                    // Loop through each key-value pair in the dictionary
                    foreach (var pair in noticesDict)
                    {
                        var noticeData = pair.Value;
                        noticeData.id = pair.Key; // Assign the key as the id
                       
                        noticeList.Add(noticeData);
                    }

                    return Ok(noticeList);
                }
                else
                {
                    return NotFound("No notices found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPut("UpdateSeen")]
        public async Task<IActionResult> UpdateSeenFromFirebase([FromBody] UpdateSeen updateSeen)
        {
            try
            {
                // Create the path to the notices node
                string nodeNotice = $"notices/customerId_{updateSeen.userId}";

                // Query data from Firebase
                FirebaseResponse response = await _client.GetAsync(nodeNotice);

                // Check if the response contains data
                if (response.Body != "null")
                {
                    // Convert the response data to a dictionary
                    Dictionary<string, NoticeData> noticesDict = JsonConvert.DeserializeObject<Dictionary<string, NoticeData>>(response.Body);

                    // Loop through each key-value pair in the dictionary
                    foreach (var pair in noticesDict)
                    {
                        if (pair.Key.Contains(updateSeen.noticeId))
                        {                                
                            var noticeData = pair.Value;
                            noticeData.id = pair.Key;
                            noticeData.seen = true;
                            string nodeNoticeUpdate = $"notices/customerId_{updateSeen.userId}/{updateSeen.noticeId}";
                            await _client.SetAsync(nodeNoticeUpdate, noticeData);
                        }
                    }

                    return Ok();
                }
                else
                {
                    return NotFound("No notices found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        private string ExtractUserId(string inputString)
		{
			string[] parts = inputString.Split('_');
			return parts[parts.Length - 1];
		}
		private string timeSend()
		{
			DateTime currentTime = DateTime.Now;
			string dateString = currentTime.ToString("yyyy-MM-dd"); // Ngày tháng năm
			string timeString = currentTime.ToString("HH:mm:ss"); // Giờ phút giây
			return dateString + " " + timeString;
		}
	}
}
