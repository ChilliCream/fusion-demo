using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Reviews.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 4, "Margaret Hamilton" },
                    { 5, "Dennis Ritchie" },
                    { 6, "Barbara Liskov" },
                    { 7, "Linus Torvalds" },
                    { 8, "Donald Knuth" },
                    { 9, "Edsger Dijkstra" },
                    { 10, "Katherine Johnson" },
                    { 11, "Tim Berners-Lee" },
                    { 12, "Frances Allen" },
                    { 13, "Ken Thompson" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "AuthorId", "Body", "CreateAt", "ProductId", "Stars" },
                values: new object[,]
                {
                    { 5, 4, "The Table went together without wobbling, and the top has a warm matte finish. It fits four place settings more comfortably than I expected.", ReviewDate(0, 9, 17), 1, 4 },
                    { 6, 5, "Our Table arrived with clean edges and solid hardware. The listed width was accurate enough for the nook by the window.", ReviewDate(6, 18, 42), 1, 4 },
                    { 7, 6, "The Table feels heavy once assembled and has held up to daily work lunches. For the price, the sturdy legs were the deciding factor.", ReviewDate(12, 11, 8), 1, 5 },
                    { 8, 7, "The Couch cushions are firm for the first week but softened nicely. Delivery was careful, and it sits exactly as deep as the dimensions suggest.", ReviewDate(18, 14, 31), 2, 5 },
                    { 9, 8, "I like the low, supportive back on this Couch and the fabric has not snagged with two cats around. It was expensive, but it looks and feels substantial.", ReviewDate(24, 20, 5), 2, 5 },
                    { 10, 9, "The Chair is neat and compact, though the seat is a little firmer than I wanted for long dinners. Assembly took fifteen minutes.", ReviewDate(30, 8, 54), 3, 3 },
                    { 11, 10, "This Chair tucks under our desk well and the finish matches the photos. The frame has stayed quiet instead of creaking.", ReviewDate(36, 16, 19), 3, 4 },
                    { 12, 11, "The Chair has a simple shape, good lumbar support, and no loose screws after a month. It feels like fair value.", ReviewDate(42, 12, 47), 3, 5 },
                    { 13, 12, "The Bookshelf shelves were easy to level, even on an old floor. The darker wood finish hides the wall behind it nicely.", ReviewDate(48, 19, 26), 4, 5 },
                    { 14, 13, "This Bookshelf is narrower than my old unit but holds paperbacks without bowing. I only wish the back panel were a little thicker.", ReviewDate(54, 10, 13), 4, 4 },
                    { 15, 1, "The Bookshelf instructions skipped one orientation detail, so I had to redo a shelf. Once built, it is steady and looks good.", ReviewDate(60, 15, 38), 4, 3 },
                    { 16, 2, "We anchored the Bookshelf to the wall and it feels very secure. The adjustable shelves made room for taller art books.", ReviewDate(66, 21, 7), 4, 5 },
                    { 17, 4, "The Bookshelf arrived without dents, which was a relief for a tall package. Its finish is smooth rather than plasticky.", ReviewDate(72, 9, 45), 4, 4 },
                    { 18, 5, "This Bookshelf has held a surprising amount of cookbooks and still sits square. The price felt reasonable for the weight of it.", ReviewDate(78, 17, 22), 4, 4 },
                    { 19, 6, "The Desk surface gives me enough room for two monitors without feeling oversized. Cable assembly underneath took patience but was worth it.", ReviewDate(84, 13, 56), 5, 5 },
                    { 20, 7, "This Desk is solid and the drawers slide cleanly. The listed depth leaves just enough room for my chair in a small office.", ReviewDate(90, 20, 11), 5, 4 },
                    { 21, 8, "The Desk finish looks better in person, but one predrilled hole needed widening. It has been stable since I tightened everything.", ReviewDate(96, 8, 33), 5, 4 },
                    { 22, 9, "I wish the Desk came with clearer drawer instructions. It is functional, although the thin back panel makes it feel less premium.", ReviewDate(102, 16, 58), 5, 3 },
                    { 23, 10, "The Desk arrived sooner than quoted and survived the trip without a scratch. It feels like a practical buy for the listed price.", ReviewDate(108, 11, 24), 5, 5 },
                    { 24, 11, "The Bed Frame has no squeaks so far, even with a heavy mattress. The slats lined up easily and the low profile matches the measurements.", ReviewDate(114, 18, 49), 6, 5 },
                    { 25, 12, "This Bed Frame took two people to move upstairs, but assembly was straightforward. The upholstered edge is softer than expected when making the bed.", ReviewDate(120, 9, 6), 6, 4 },
                    { 26, 13, "The Bed Frame feels well braced and did not need a box spring. I would have liked labels on the side rails.", ReviewDate(126, 14, 42), 6, 4 },
                    { 27, 1, "Our Bed Frame arrived with one scuffed corner hidden against the wall. It remains quiet and sturdy after several weeks.", ReviewDate(132, 21, 15), 6, 3 },
                    { 28, 2, "The Bed Frame looks clean and modern without taking up extra floor space. The center support is reassuring on a larger mattress.", ReviewDate(138, 10, 27), 6, 4 },
                    { 29, 4, "I assembled the Bed Frame alone in under an hour. The finish is even, and the frame does not shift when getting in or out.", ReviewDate(144, 17, 53), 6, 5 },
                    { 30, 5, "The Nightstand drawer is deeper than it appears in the photo, so it holds chargers and a book. Its height lines up well with our mattress.", ReviewDate(150, 8, 16), 7, 4 },
                    { 31, 6, "This Nightstand has a nice grain and no sharp corners. The drawer sticks slightly at the last inch, but not enough to return it.", ReviewDate(156, 15, 41), 7, 3 },
                    { 32, 7, "The Nightstand was already mostly assembled and feels more substantial than flat-pack furniture. It is a handsome match beside the bed.", ReviewDate(162, 19, 9), 7, 5 },
                    { 33, 8, "I measured first and the Nightstand fits the narrow gap perfectly. The small shelf keeps my glasses from disappearing under the bed.", ReviewDate(168, 11, 34), 7, 4 },
                    { 34, 9, "The Nightstand packaging was thoughtful and everything arrived intact. It is simple, stable, and appropriately priced.", ReviewDate(174, 20, 2), 7, 4 },
                    { 35, 10, "The Coffee Table has a smooth top that wipes clean after snacks and board games. The lower shelf is useful without making the room feel crowded.", ReviewDate(180, 9, 28), 8, 5 },
                    { 36, 11, "This Coffee Table is sturdy once assembled, although aligning the lower shelf took a second try. The dimensions suit a standard sofa well.", ReviewDate(186, 16, 52), 8, 4 },
                    { 37, 12, "The Coffee Table arrived with a small scratch on the underside and a chipped corner. It works, but the finish did not justify the price.", ReviewDate(192, 12, 14), 8, 2 },
                    { 38, 13, "I like the rounded corners on this Coffee Table with toddlers nearby. It has enough weight that it does not slide across the rug.", ReviewDate(198, 18, 37), 8, 5 },
                    { 39, 1, "The Coffee Table is lower than I pictured, so check the listed height before ordering. The wood tone itself is lovely.", ReviewDate(204, 10, 59), 8, 4 },
                    { 40, 2, "This Coffee Table was easy to put together and has held up through daily use. The price felt sensible for such a solid piece.", ReviewDate(210, 21, 21), 8, 4 },
                    { 41, 4, "The Dining Chair has a comfortable curve through the back and the legs sit evenly on tile. I can finish a long meal without shifting around.", ReviewDate(216, 8, 43), 9, 4 },
                    { 42, 5, "This Dining Chair looks good, but the seat padding is thin after an hour. It was simple to assemble and does not wobble.", ReviewDate(222, 15, 5), 9, 3 },
                    { 43, 6, "The Dining Chair fabric cleans easily and the stitching is tidy. We bought several and their color is consistent.", ReviewDate(228, 19, 32), 9, 5 },
                    { 44, 7, "Our Dining Chair arrived on schedule with all the hardware sorted. It feels strong and the seat height matches our table.", ReviewDate(234, 11, 57), 9, 4 },
                    { 45, 8, "The Dining Chair is a good size for a smaller dining room. I would buy it again for the price and straightforward setup.", ReviewDate(240, 20, 18), 9, 4 },
                    { 46, 9, "The Wardrobe offers useful hanging space, though the interior shelves are shallower than I expected. It took an afternoon to assemble.", ReviewDate(246, 9, 39), 10, 3 },
                    { 47, 10, "This Wardrobe looks much cleaner than an open rack and the doors close evenly. The listed height was exactly what our ceiling could handle.", ReviewDate(252, 16, 4), 10, 4 },
                    { 48, 11, "The Wardrobe panels are heavy and the finished cabinet does not sway. A second person is essential when attaching the doors.", ReviewDate(258, 12, 29), 10, 3 },
                    { 49, 12, "I appreciate the Wardrobe's adjustable rail and the quiet hinges. The white finish has stayed easy to clean.", ReviewDate(264, 18, 51), 10, 4 },
                    { 50, 13, "The Wardrobe came with every part labeled and no damaged corners. It is a plain design, but it makes the room much tidier.", ReviewDate(270, 10, 22), 10, 3 },
                    { 51, 1, "This Wardrobe is good value if you need closed storage fast. The drawers are smaller than expected, yet the cabinet feels stable.", ReviewDate(276, 21, 8), 10, 4 },
                    { 52, 2, "The TV Stand has a wide enough top for our television and a soundbar. Cable openings at the back keep the cords from becoming a mess.", ReviewDate(282, 8, 35), 11, 5 },
                    { 53, 4, "This TV Stand was easy to level and the doors line up neatly. It is a little lower than our old console, as the dimensions indicate.", ReviewDate(288, 15, 54), 11, 4 },
                    { 54, 5, "The TV Stand has a sturdy feel and the shelves hold our game console without trapping too much heat. Assembly was uneventful.", ReviewDate(294, 19, 17), 11, 4 },
                    { 55, 6, "I like the TV Stand finish, but the included wall straps were short for our setup. The cabinet itself is stable.", ReviewDate(300, 11, 46), 11, 3 },
                    { 56, 7, "The TV Stand arrived well packed and looks more expensive than it was. It gave us useful hidden storage for remotes.", ReviewDate(306, 20, 9), 11, 5 },
                    { 57, 8, "The Dresser drawers hold plenty of folded clothes and roll smoothly when full. The top is wide enough for a mirror and lamp.", ReviewDate(312, 9, 24), 12, 4 },
                    { 58, 9, "This Dresser is serviceable, but two drawer fronts needed adjustment to sit flush. The finish is pleasant once assembled.", ReviewDate(318, 16, 49), 12, 3 },
                    { 59, 10, "The Dresser feels heavy in a good way and has not racked when the drawers are open. Delivery brought it right to the door.", ReviewDate(324, 12, 12), 12, 5 },
                    { 60, 11, "I was worried the Dresser would dominate the bedroom, but its listed width fits the wall well. The handles are comfortable to pull.", ReviewDate(330, 18, 34), 12, 4 },
                    { 61, 12, "The Dresser instructions could show the drawer runners more clearly. After that hurdle, it is a solid and attractive piece.", ReviewDate(336, 10, 56), 12, 4 },
                    { 62, 13, "This Dresser has good storage for the price, though the bottom drawer is not as deep as the others. It remains smooth to use.", ReviewDate(342, 21, 3), 12, 3 },
                    { 63, 1, "The Armchair has a deep seat without swallowing the whole corner. Its fabric feels durable and the cushions recovered their shape overnight.", ReviewDate(348, 8, 27), 13, 5 },
                    { 64, 2, "This Armchair is comfortable for reading, especially with the supportive arms. The color is slightly warmer than on my monitor.", ReviewDate(354, 15, 51), 13, 4 },
                    { 65, 4, "The Armchair took only a few minutes to attach the legs and feels balanced. I wish the seat were a touch softer.", ReviewDate(360, 19, 14), 13, 4 },
                    { 66, 5, "I like the Armchair's shape, but the back cushion slides down during longer sits. It is still a good-looking accent chair.", ReviewDate(366, 11, 38), 13, 3 },
                    { 67, 6, "The Armchair arrived without any odor and fits the space shown by the listed dimensions. It feels sturdy for daily use.", ReviewDate(372, 20, 1), 13, 4 },
                    { 68, 7, "The Bar Stool has a stable footrest and the seat height works at our island. I was able to assemble it with the included wrench.", ReviewDate(378, 9, 32), 14, 4 },
                    { 69, 8, "This Bar Stool looks nice but the seat is too firm for lingering over coffee. The base is steady and does not scratch the floor.", ReviewDate(384, 16, 55), 14, 3 },
                    { 70, 9, "The Bar Stool swivels smoothly and the upholstery has held up to spills. It feels more substantial than the price suggested.", ReviewDate(390, 12, 18), 14, 5 },
                    { 71, 10, "Our Bar Stool arrived with a tiny mark on one leg, but it was easy to hide. The height adjustment works reliably.", ReviewDate(396, 18, 43), 14, 4 },
                    { 72, 11, "The Bar Stool was awkward to put together because the bolt holes were tight. Once finished, it is comfortable enough for quick meals.", ReviewDate(402, 10, 7), 14, 2 },
                    { 73, 12, "This Bar Stool has a clean profile and the footrest is placed well. It is a sensible choice for a narrow kitchen.", ReviewDate(408, 21, 25), 14, 4 },
                    { 74, 13, "The Sideboard gives us much-needed serving storage, and the doors close without rubbing. The wood tone works well with our older table.", ReviewDate(414, 8, 48), 15, 5 },
                    { 75, 1, "This Sideboard took some careful assembly, especially the door alignment. The finished cabinet is sturdy and the shelves are useful.", ReviewDate(420, 15, 11), 15, 4 },
                    { 76, 2, "The Sideboard looks good from across the room, but the drawer bottoms feel light for heavy dishes. Fine for linens and smaller items.", ReviewDate(426, 19, 36), 15, 3 },
                    { 77, 4, "I measured the Sideboard against the listed dimensions and it fits our dining wall exactly. The surface is easy to wipe after serving meals.", ReviewDate(432, 11, 58), 15, 4 },
                    { 78, 5, "The Sideboard arrived in excellent condition and feels weighty once assembled. It adds storage without looking bulky.", ReviewDate(438, 20, 14), 15, 5 },
                    { 79, 6, "The Bench has a straightforward build and does not rock on our hardwood floor. Its length seats two adults comfortably at the table.", ReviewDate(444, 9, 41), 16, 4 },
                    { 80, 7, "This Bench is useful at the entry, although the finish picked up a small scuff quickly. The frame itself feels dependable.", ReviewDate(450, 16, 6), 16, 3 },
                    { 81, 8, "The Bench is sturdier than it looks and the cushion has just enough give. It fits under our window using the listed depth.", ReviewDate(456, 12, 31), 16, 5 },
                    { 82, 9, "I like the clean lines of this Bench and the hardware was neatly separated. It is comfortable for a short sit while putting on shoes.", ReviewDate(462, 18, 54), 16, 4 },
                    { 83, 10, "The Bench arrived late and one leg had a rough patch in the stain. It is usable, but the quality control should be better.", ReviewDate(468, 10, 25), 16, 3 },
                    { 84, 11, "This Bench feels solid enough for daily use and was easy to carry through a narrow hallway. The price seems fair.", ReviewDate(474, 21, 9), 16, 4 },
                    { 85, 12, "The Rocking Chair has a smooth, quiet motion and a surprisingly supportive back. It was nearly assembled out of the box.", ReviewDate(480, 8, 36), 17, 5 },
                    { 86, 13, "This Rocking Chair is comfortable, though the armrests are narrower than I expected. The wood finish is even and warm.", ReviewDate(486, 15, 57), 17, 4 },
                    { 87, 1, "The Rocking Chair sits lower than our other chair, so test the listed seat height. It rocks steadily but needs a small cushion.", ReviewDate(492, 19, 21), 17, 3 },
                    { 88, 2, "I use the Rocking Chair every evening and the runners have not marked the floor. The joints feel tight and well made.", ReviewDate(498, 11, 44), 17, 4 },
                    { 89, 4, "The Rocking Chair was packed carefully and had no chips or loose hardware. It is comfortable enough to make the price worthwhile.", ReviewDate(504, 20, 6), 17, 5 },
                    { 90, 5, "This Rocking Chair has a lovely shape, but the included floor pads came off quickly. The chair itself remains stable.", ReviewDate(510, 9, 29), 17, 4 },
                    { 91, 6, "The Storage Cabinet holds cleaning supplies neatly, and the doors do not swing open on their own. It fits the utility room as measured.", ReviewDate(516, 16, 53), 18, 4 },
                    { 92, 7, "This Storage Cabinet took longer to assemble than expected because several panels look alike. It is useful, but the shelves are light duty.", ReviewDate(522, 12, 16), 18, 3 },
                    { 93, 8, "The Storage Cabinet has adjustable shelves that made room for taller bottles. Once anchored, it feels very secure.", ReviewDate(528, 18, 38), 18, 5 },
                    { 94, 9, "I appreciate the Storage Cabinet's narrow depth in our laundry room. The finish has handled damp towels without swelling.", ReviewDate(534, 10, 2), 18, 4 },
                    { 95, 10, "The Storage Cabinet arrived with one dented shelf and the replacement took a while. The rest of the cabinet is sturdy.", ReviewDate(540, 21, 19), 18, 3 },
                    { 96, 11, "This Storage Cabinet offers more usable space than expected for its footprint. The doors align well and the price was reasonable.", ReviewDate(546, 8, 47), 18, 4 }
                });

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Users\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Users\"), 1), true);");
            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Reviews\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Reviews\"), 1), true);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
                    25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44,
                    45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64,
                    65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
                    85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96
                });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValues: new object[] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 });

            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Users\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Users\"), 1), true);");
            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('\"Reviews\"', 'Id'), COALESCE((SELECT MAX(\"Id\") FROM \"Reviews\"), 1), true);");
        }

        private static DateTimeOffset ReviewDate(int days, int hour, int minute)
            => new(new DateTime(2025, 1, 2, hour, minute, 0, DateTimeKind.Utc).AddDays(days));
    }
}
