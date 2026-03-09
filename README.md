# Dự Án Mini 2D Unity -- Trò Chơi Khai Thác Vàng

## Mục Tiêu Dự Án

Tạo một bản nguyên mẫu nhỏ của trò chơi kiểu Gold Miner bằng Unity 2D.\
Dự án chỉ cần:

-   Một **Menu Chính** cơ bản
-   **1-2 cấp độ có thể chơi được**
-   **Cơ chế lure (móc) cốt lõi**
-   **Hệ thống điểm số và bộ đếm thời gian**
-   **Điều kiện chiến thắng / thua cơ bản**

Mục đích là kiểm tra cơ chế gameplay và cấu trúc dự án.

------------------------------------------------------------------------

# Phương Pháp Phát Triển

Dự án này tuân theo **quy trình Kanban**:

Backlog → To Do → In Progress → Testing → Done

Các nhiệm vụ di chuyển từ trái sang phải cho đến khi tính năng hoàn thành.

------------------------------------------------------------------------

# Bảng Kanban

## Backlog

### Gameplay

-   Thiết kế gameplay Gold Miner cốt lõi
-   Hệ thống xoay móc
-   Cơ chế bắn móc
-   Cơ chế móc trở về
-   Hệ thống bắt đối tượng
-   Trọng lượng ảnh hưởng tốc độ kéo
-   Hệ thống điểm số
-   Bộ đếm thời gian cấp độ

### Đối Tượng

-   Đối tượng vàng
-   Đối tượng đá
-   Các giá trị khác nhau cho các đối tượng

### Cấp Độ

-   Thiết kế Cấp Độ 1
-   Thiết kế Cấp Độ 2
-   Vị trí sinh ngẫu nhiên
-   Điểm số mục tiêu cho mỗi cấp độ

### Giao Diện

-   Menu Chính
-   Nút Bắt Đầu Trò Chơi
-   Nút Thoát
-   HUD Gameplay
-   Hiển thị Điểm Số
-   Hiển thị Bộ Đếm Thời Gian
-   Hiển thị Điểm Số Mục Tiêu
-   Màn Hình Chiến Thắng
-   Màn Hình Thua Cuộc

### Hiệu Ứng Hình Ảnh

-   Sprite Người Chơi
-   Sprite Móc
-   Sprite Vàng
-   Sprite Đá
-   Hoạt Cảnh Móc
-   Hiệu Ứng Âm Thanh

### Hệ Thống Trò Chơi

-   GameManager
-   LevelManager
-   ScoreManager
-   TimerManager
-   Quản lý Cảnh

------------------------------------------------------------------------

## To Do

### Thiết Lập Dự Án

-   Tạo dự án Unity 2D
-   Tạo cấu trúc thư mục

Assets/ Scripts/ Prefabs/ Scenes/ Sprites/ Audio/ UI/

-   Tạo cảnh
    -   MainMenu
    -   Level1
    -   Level2

### Hệ Thống Người Chơi

-   Tạo script HookController
-   Tạo logic xoay móc
-   Thêm phát hiện đầu vào (Space / Mouse)

### Hệ Thống Đối Tượng

-   Tạo prefab vàng
-   Tạo prefab đá
-   Thêm các biến giá trị và trọng lượng

------------------------------------------------------------------------

## In Progress

### Gameplay Móc

-   Móc xoay trái và phải
-   Người chơi nhấn đầu vào để bắn móc
-   Móc va chạm với đối tượng
-   Đối tượng gắn vào móc
-   Móc kéo đối tượng trở lại

### Logic Đối Tượng

Mỗi đối tượng chứa: - giá trị - trọng lượng

Tốc độ kéo bị ảnh hưởng bởi trọng lượng đối tượng.

### Hệ Thống Điểm Số

-   Thêm điểm khi đối tượng tiếp cận người chơi

### Hệ Thống Bộ Đếm Thời Gian

-   Bộ đếm ngược
-   Kết thúc cấp độ khi bộ đếm thời gian về 0

------------------------------------------------------------------------

## Testing

-   Kiểm tra va chạm móc
-   Kiểm tra sự khác biệt tốc độ kéo
-   Kiểm tra cập nhật điểm số
-   Kiểm tra chức năng bộ đếm thời gian
-   Kiểm tra chuyển đổi cấp độ
-   Kiểm tra điều kiện chiến thắng / thua

Kiểm tra gameplay: - Cân bằng đặt đối tượng - Điều chỉnh độ khó cấp độ

------------------------------------------------------------------------

## Done

Bản nguyên mẫu được coi là hoàn chỉnh khi:

-   Menu chính hoạt động
-   Gameplay móc hoạt động
-   Hệ thống điểm số hoạt động
-   Hệ thống bộ đếm thời gian hoạt động
-   1-2 cấp độ có thể chơi được
-   Điều kiện chiến thắng / thua hoạt động
-   Trò chơi có thể được xây dựng và chạy

------------------------------------------------------------------------

# Dòng Thời Gian Phát Triển Đề Xuất

### Ngày 1

-   Thiết lập dự án
-   Hệ thống xoay móc

### Ngày 2

-   Bắn móc và kéo
-   Hệ thống đối tượng

### Ngày 3

-   Hệ thống điểm số
-   Hệ thống bộ đếm thời gian

### Ngày 4

-   Cấp Độ 1 và Cấp Độ 2

### Ngày 5

-   Menu giao diện
-   Kiểm tra và hoàn thiện

------------------------------------------------------------------------

# Cấu Trúc Script Đề Xuất
```
Scripts/

GameManager.cs\
LevelManager.cs\
HookController.cs\
HookMovement.cs\
CollectibleObject.cs\
ScoreManager.cs\
TimerManager.cs
```
------------------------------------------------------------------------

# Sản Phẩm Tối Thiểu Khả Thi (MVP)

Phiên bản demo phải bao gồm:

-   Menu chính
-   1-2 cấp độ có thể chơi được
-   Cơ chế móc bắt
-   Hệ thống điểm số
-   Hệ thống bộ đếm thời gian
-   Màn hình chiến thắng và thua

