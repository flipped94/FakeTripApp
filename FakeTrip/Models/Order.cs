using Stateless;
using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;

namespace FakeTrip.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public ICollection<LineItem> OrderItems { get; set; }
    public OrderStateEnum State { get; set; }
    public DateTime CreateDateUTC { get; set; }
    public string TransactionMetadata { get; set; }

    StateMachine<OrderStateEnum, OrderStateTriggerEnum> machine;


    public Order()
    {
        StateMachineInit();
    }

    private void StateMachineInit()
    {
        machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>
        (OrderStateEnum.Pending);

        machine.Configure(OrderStateEnum.Pending)
            .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing)
            .Permit(OrderStateTriggerEnum.Cancel, OrderStateEnum.Cancelled);

        machine.Configure(OrderStateEnum.Processing)
            .Permit(OrderStateTriggerEnum.Approve, OrderStateEnum.Completed)
            .Permit(OrderStateTriggerEnum.Reject, OrderStateEnum.Declined);

        machine.Configure(OrderStateEnum.Declined)
            .Permit(OrderStateTriggerEnum.PlaceOrder, OrderStateEnum.Processing);

        machine.Configure(OrderStateEnum.Completed)
            .Permit(OrderStateTriggerEnum.Return, OrderStateEnum.Refund);
    }

}

public enum OrderStateEnum
{
    Pending, // 订单已生成
    Processing, // 支付处理中
    Completed, // 交易成功
    Declined, // 交易失败
    Cancelled, // 订单取消
    Refund, // 已退款
}

public enum OrderStateTriggerEnum
{
    PlaceOrder, // 支付
    Approve, // 收款成功
    Reject, // 收款失败
    Cancel, // 取消
    Return // 退货
}
