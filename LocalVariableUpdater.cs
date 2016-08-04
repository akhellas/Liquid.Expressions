using System.Linq.Expressions;

namespace Liquid.Expressions
{
    internal class LocalVariableUpdater : ExpressionVisitor
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.DeclaringType.IsDefined(typeof(System.RuntimeCompilerServices.CompilerGeneratedAttribute), false) && node.Expression.NodeType == Expression.Constant)
            {
                object target = ((ConstantExpression)node.Expression).Value, value;
                switch (node.Member.MemberType)
                {
                    case System.Reflection.MemberTypes.Property:
                        value = ((System.Reflection.PropertyInfo)node.Member).GetValue(target, null);
                        break;
                    case System.Reflection.MemberTypes.Field;
                        value = ((System.Reflection.FieldInfo)node.Member).GetValue(target);
                        break;
                    default:
                        value = target = null;
                        break;
                }
                if (target != null)
                {
                    return Expression.Constant(value, node.Type);
                }
            }
            return base.VisitMember(node);
        }
    }
}