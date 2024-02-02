using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfoGatherer.api.Controllers
{
    public class TestController : BaseController
    {

        public class Item
        {
            public int Id { get; set; }
            public string Name { get; set; }
        };
        private static List<Item> _items = new List<Item>()
        {
            new Item() { Id = 1, Name = "Item 1" },
            new Item() { Id = 2, Name = "Item 2" }
        };
        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<Item> GetItem(int id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        [HttpPost]
        public ActionResult<Item> CreateItem(Item item)
        {
            _items.Add(item);
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateItem(int id, Item item)
        {
            var index = _items.FindIndex(i => i.Id == id);
            if (index == -1)
            {
                return NotFound();
            }
            _items[index] = item;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteItem(int id)
        {
            var index = _items.FindIndex(i => i.Id == id);
            if (index == -1)
            {
                return NotFound();
            }
            _items.RemoveAt(index);
            return NoContent();
        }
    }

}
